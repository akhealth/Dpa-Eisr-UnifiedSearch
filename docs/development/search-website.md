# Search Website

## React

### State

#### Background

At the time of its release, React was unique, compared to most client-side frameworks, due to its narrow focus on the _view_ portion of web applications, i.e., the `V` in MVC/MVP/MV*/etc. Among other reasons, this is advantageous because it allows developers choice in the libraries or techniques used to manage state in their applications.

Traditionally, React apps used a state management pattern known as _Flux_, characterized by a unidirctional data flow and several "stores" that contain the state for different parts of the application. User interaction, network activity, and other events update the stores, which initiates a re-render of the relevant parts of the application. This one-way data-flow paradigm was extremely useful in organizing projects and making them easier to reason about.

However, Flux is more of an architecture than a library, and largely leaves the implementation up to the user. After some time, a number of Flux-compatible libraries emerged to fill this gap, such as Redux, a library inspired by the Elm architecture. Redux uses a central dispatcher and `actions` to decouple and organize state modifications. However, in spite of its many potential advantages, Redux tends to be very verbose, requiring a lot of boilerplate, and has a steep learning curve for new developers.

There must be a better way...

#### Baobab

Like Redux, Baobab is a library for tracking state changes in applications. Unlike Redux, Baobab has very little boilerplate and has no central dispatch mechanism, which makes it easier to learn and use, especially for smaller projects.

A baobab state tree creation looks like this:

```jsx
import Baobab from 'baobab'
const stateTree = new Baobab({ title: 'my awesome application' })
```

Generally, there's only one state tree that captures the complete state of the application, much like Redux and Elm. Baobab is immutable by default, so we have to get and set state using methods on the tree, e.g.:

```jsx
stateTree.set('title', 'a new title for my application')
```

##### `root` & `branch`

Baobab and React go together well, and even moreso with the help of the `baobab-react` library. We use two higher-order components, `root` and `branch`, to connect parts of the state tree to our React components.

The `root` function is used on the root app component to provide the state tree to child components. Those child components use the `branch` function to request parts of the tree, which are passed in like normal props:

```jsx
const MyComponent = ({ title }) => {
    return (
        <h1>{ title }</h1>
    )
}

export default branch({
    title: [ 'title' ]
}, MyComponent)
```

One nice side effect of this, aside from being convenient, is that changes to the selected data will cause a re-render of the branched component, which largely absolves us from manually configuring when components should update.

###### When and why to use `branch`

There are some tradeoffs with using `branch`--while convenient, generally, most components should _not_ use `branch` to acquire props. Using `branch` tightly couples components to the state tree, which can make the application state changes more difficult to reason about and decreases component re-usability.

On the other hand, `branch` helps prevent re-rendering more components than is necessary, which can make the UI more performant.

**A Suggestion**: Generally, try to use `branch` only in page components (`web/client/pages/`) and not in regular components. These page components will handle the transformation of data in the tree and pass down what's necessary to child components. This is similar to the concept of "container components" vs. "dumb components" in React & Redux apps.

### Validation

We implemented our own validation logic rather than using a third-party library. This gave more control over how and when validation errors are displayed to the user.

In general, the strategy is to keep validation information in the client-side state tree, `baobab`, and pass it to components that know how to render themselves with or without the errors.

For example, say we have a text input for a social security number in a search page component:

```jsx 
const SsnInput = () => {
    return (
        <div id='ssn-input-group'>
            <label>SSN</label>
            <input type='text' name='ssn'/>
        </div>
    )
}
```

There are a few tricky parts here. First, we need a way to test the input to see if it's a valid SSN. Then, we need a way to display any errors to the user. However, should we show the error immediately, as the user's typing? On one hand, fast feedback might be useful, but displaying a distracting red error message after only a single keystroke seems like overkill.

First, though, lets add an error message to the input:

```jsx 
const SsnInput = ({ errorMessage }) => {
    return (
        <div id='ssn-input-group'>
            <label>SSN</label>
            <input type='text' name='ssn'/>
            { errorMessage ? <span className='error'>{ errorMessage }</span> : false }
        </div>
    )
}
```

Now, when an `errorMessage` prop is passed to our `SsnInput` component, we should see the message render below our input. But where did the message come from? Some parent component likely passed it in, but we need to populate it in the first place.

One option is to create an `action` to set validation errors directly in the component:

```jsx 
const SsnInput = ({ errorMessage, setError }) => {
    const validate = ({ target }) => {
        if (!isValidSsn(target.value)) {
            setError('Input is not a valid SSN')
        }
    }
    return (
        <div id='ssn-input-group'>
            <label>SSN</label>
            <input type='text' name='ssn' onChange={ validate }/>
            { errorMessage ? <span className='error'>{ errorMessage }</span> : false }
        </div>
    )
}
```

This component now contains its own validation logic, and can set external state using the `setError` function we passed in. This `setError` function could do anything we want, but one possibilty might be to set a field in our baobab tree causing the input component (or one its parents) to re-render.

If we wanted to hide the error intially, we can pass in one more parameter:

```jsx 
const SsnInput = ({ showError, errorMessage, setError }) => {
    const validate = ({ target }) => {
        if (target.value !== '' && !isValidSsn(target.value)) {
            setError('Input is not a valid SSN')
        }
    }
    return (
        <div id='ssn-input-group'>
            <label>SSN</label>
            <input type='text' name='ssn' onChange={ validate }/>
            { (showError && errorMessage) ? <span className='error'>{ errorMessage }</span> : false }
        </div>
    )
}
```

Notice at the bottom, we check if we should show the error and if there is an error message. If we wanted to wait until the user tried to submit the form, then we could set `showError` to true when they click the `submit` button. If this component was then re-rendered, `showError` would cause the error `<span>` to display.