// Enable new built-ins like Promise or WeakMap, static methods like Array.from or Object.assign,
// instance methods like Array.prototype.includes, and generator functions.
// https://babeljs.io/docs/usage/polyfill/
import 'babel-polyfill'

import React from 'react'
import { render } from 'react-dom'
import { BrowserRouter as Router } from 'react-router-dom'

import '-/styles/global.scss'
import Application from '-/components/Application'
import state from '-/state'

render(
  <Router basename={ state.get([ 'config', 'basename' ]) }>
    <Application />
  </Router>,
  document.getElementById('app')
)
