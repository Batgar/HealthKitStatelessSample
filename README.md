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




In the overall Stateless design there are the following main actors:

* Store -- Where the state to show within the app is sourced from.
* State -- Contains chunks of state from 1 or more Stores
* View -- Where the State is to be received and only displayed.
* Dispatcher -- Handles action processing from the view (and other locations) and notifies the store to manage the state. In this solution it also distributes State objects to the views.

In this code, we have 1 static store: HealthKitDataStore

From this Store we source 3 different states:
* `BloodGlucoseRecommendationState`
* `HealthState`
* `BloodGlucoseEntryListState` which contains 1 or more `BloodGlucoseEntryState` entries.

We use a fully bootstrapped and static StateDispatcher<T> where T is guaranteed to be an IState type as a safeguard.

We also use a non-static ListStateDispatcher<T,S> where T is guaranteed to be an IState type, and S is a specialized IListStore<T> interface that can handle the dispatcher to store communication of list changes.

Let's start with code in the View Controllers, then drill on down to the other stuff.

The hard stuff is the handling of List<T> through a UITableViewController implementation.

The easy stuff is the one to one mapping of properties to standard appearance of data in a UILabel.

Let's start with the UILabel mappings in a view of the HealthState and BloodGlucoseRecommendationState data.

Take a look at ViewController.Health.View:

The first thing to notice is that we use partial classes to declutter the standard ViewController stuff and separate out the code that renders our HealthState object into the ViewController.

```partial class ViewController : IView<HealthState>
{
	public void Receive(HealthState state)
	{
		this.weight.Text = state.Height.ToString();
		this.bloodGlucose.Text = state.BloodGlucose.ToString ();
		this.biologicalSex.Text = state.BiologicalSex;
	}
}```

So we have clean code that maps HealthState members into our UILabels backed by our view controller, but how is this code actually hit?

To see how it all connects up, take a look at ViewController.Binding.cs:
```using System;
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
}```	

Notice that we bind to a completely static StateDispatcher<T> for 2 different state types: HealthState and BloodGlucoseRecommendationState.

The beauty of these being completely static is that since we have a completely static top-level data source as well: HealthKitDataStore.

There is really no need to have any instance variables anywhere in the application. We simply use static types across the whole system to indicate what we want.

The StateDispatcher<T> also tracks the single app wide instance of the state object type T.

The Bind method on each StateDispatcher<T> assumes that the ViewController has implemented IView<T>, and the IView<T> is just stored in a list to be used later for broadcasts of state updates.

The Bind method also does an initial blanket IView<T>.Refresh(T) call on Bind to ensure that initial state is populated into the view.

The Unbind method on each State object simply removes the IView<T> instance from a stored list.


Let's dig deeper and see how state updates occur.

`
//Pull out the HealthState object.
var healthState = StateDispatcher<HealthState>.State;

//Update the BloodGlucose member on the single HealthState object within the app.
var mgPerDL = HKUnit.FromString("mg/dL");
healthState.BloodGlucose = lastBloodGlucoseQuantity.GetDoubleValue(mgPerDL);


/At this point all IView<T> based subscribers bound to the dispatcher will update.
StateDispatcher<HealthState>.Refresh();`

... and that is really it. 

Lists and UITableViews are a little more complicated.



