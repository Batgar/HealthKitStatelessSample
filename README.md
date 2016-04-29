#HealthKitStatelessSample

Introduction
============

Thanks to a recent minnebar presentation on Stateless UIs in Swift by Sam Kirchmeier and Adam May I decided to take on their design concept within Xamarin.

Before proceeding really grok the overall slide deck on [Stateless UIs in Swift](https://speakerdeck.com/skirchmeier/exploring-stateless-uis-in-swift) from Sam Kirchmeier and Adam May.

I focused on a sample Xamarin.iOS HealthKit app that I was working on to look at HealthKit data integration from third party apps.

Future -- Will be seeing how well the design will flex to work against Google Fit via a Mono.Android app.


Outcomes
========

In general, I think the Stateless design concepts from Sam and Adam created something that is beyond my wildest dreams of code cleanliness at the top levels.

I had to make some interesting compromises at the data end of the solution to allow for HealthKit quirks, and to allow for future adaptability into Google Fit, but I really like the overall solution.


Highlights
==========

Please see the [overall architecture document](https://github.com/Batgar/HealthKitStatelessSample/blob/master/Stateless%20Cross-Platform%20Health%20Data%20Infrastructure.pdf) for 1 (of many) state objects. It should help serve as a guide as to where to look in the code.


In the overall Stateless design there are the following main actors:

* Store -- Where the state to show within the app is sourced from.
* State -- Contains chunks of state from 1 or more Stores
* View -- Where the State is to be received and only displayed.
* Dispatcher -- Handles action processing from the view (and other locations) and notifies the store to manage the state. In this solution it also distributes State objects to the views.

I don't like the compromises made from the original 'stateless' design. The key compromise is the overloading of the Dispatcher as a State container, hence the name `StateDispatcher`. But it seemed like a really easy compromise to make given the lifetime and bootstrapping done throughout the solution.

In this code, we have 1 static store: HealthKitDataStore

From this Store we source 3 different states:
* `BloodGlucoseRecommendationState`
* `HealthState`
* `BloodGlucoseEntryListState` which contains 1 or more `BloodGlucoseEntryState` entries.

We use a fully bootstrapped and static `StateDispatcher<T>` where T is guaranteed to be an `IState` type as a safeguard.

We also use a non-static `ListStateDispatcher<T,S>` where T is guaranteed to be an `IState` type, and S is a specialized `IListStore<T>` interface that can handle the communication between the dispatcher and a list store to handle list changes.

Let's start with code in the View Controllers, then drill on down to the other stuff.

The hard stuff is the handling of List<T> through a UITableViewController implementation. We don't cover that in this README, yet.

The easy stuff is the one to one mapping of properties to standard appearance of data in a UILabel via an IView / UIViewController.

Let's start with the UILabel mappings in a view of the `HealthState` and `BloodGlucoseRecommendationState` data.

Take a look at ViewController.Health.View:

The first thing to notice is that we use partial classes to declutter the standard ViewController stuff and separate out the code that renders our HealthState object into the ViewController.

```C#
partial class ViewController : IView<HealthState>
{
	public void Receive(HealthState state)
	{
		this.weight.Text = state.Height.ToString();
		this.bloodGlucose.Text = state.BloodGlucose.ToString ();
		this.biologicalSex.Text = state.BiologicalSex;
	}
}
```

So we have clean code that maps HealthState members into our UILabels backed by our view controller, but how is this code actually hit?

To see how it all connects up, take a look at ViewController.Binding.cs:
```C#
using System;
using Stateless;

namespace HealthKitSample
{
	partial class ViewController
	{
		private void Bind()
		{
			StateDispatcher<HealthState>.Bind(this);
			StateDispatcher<BloodGlucoseRecommendationState>.Bind (this);
		}

		private void Unbind()
		{
			StateDispatcher<HealthState>.Unbind(this);
			StateDispatcher<BloodGlucoseRecommendationState>.Unbind (this);
		}
	}
}
```	

Notice that we bind to a completely static `StateDispatcher<T>` for 2 different state types: `HealthState` and `BloodGlucoseRecommendationState`.

The beauty of `StateDispatcher<T>` being completely static is that we have a completely static top-level data source in the form of `HealthKitDataStore`.

There is really no need to have any instance variables anywhere in the application. We simply use static types across the whole system to indicate what specific states we want.

The `StateDispatcher<T>` also tracks the single app wide instance of the state object type T.

The `Bind` method on each `StateDispatcher<T>` assumes that the `ViewController` has implemented `IView<T>`, and the `IView<T>` is just stored in a list to be used later for broadcasts of state updates.

The `Bind` method also does an initial blanket `IView<T>.Refresh(T state)` call on Bind to ensure that initial state is populated into the view.

The `Unbind` method on each State object simply removes the `IView<T>` instance from a stored list.


Let's dig deeper and see how state updates occur, in this case from the `HealthKitDataStore`.

```C#
//Pull out the HealthState object.
var healthState = StateDispatcher<HealthState>.State;

//Update the BloodGlucose member on the single HealthState object within the app.
//We use a special mutator object because the properties on HealthState are marked with
//'internal set' to ensure that the properties cannot be mutated outside of any other assembly
//than the original assembly. This is a 'pseudo-hack' to ensure mutation only occurs in one location.
var mgPerDL = HKUnit.FromString("mg/dL");
HealthStateMutator.MutateBloodGlucose(healthState, () => lastBloodGlucoseQuantity.GetDoubleValue(mgPerDL));


//At this point all IView<T> based subscribers bound to the dispatcher will update.
StateDispatcher<HealthState>.Refresh();
```



Lists and UITableViews are a little more complicated. More on that later.

I also want this scheme to work across Mono.Android projects, so we will rev an example there and see how it works.

Google Fit data will have to be used instead of HealthKit, so it is going to be interesting to swap out the bottom end and the top end and see how it all could possibly gel.



