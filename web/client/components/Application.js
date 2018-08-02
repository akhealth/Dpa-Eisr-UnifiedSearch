import React from 'react'
import { root } from 'baobab-react/higher-order'
import { Route, Switch, withRouter } from 'react-router-dom'

import initialState from '-/state'
import { setHistory } from '-/actions/session'
import Header from '-/components/Header/Header'
import IdleMonitor from '-/components/IdleMonitor/IdleMonitor'
import Search from '-/pages/Search/Search'
import Person from '-/pages/Person/Person'
import EndSession from '-/pages/EndSession/EndSession'
import NotFound from '-/pages/NotFound/NotFound'
import LogoutSuccess from '-/pages/LogoutSuccess/LogoutSuccess'
import ErrorBoundary from '-/components/ErrorBoundary/ErrorBoundary'

const Application = withRouter(({ history }) => {
  if (history) {
    setHistory(history)
  }
  return (
    <div id="app-component">
      <Header />
      <main id="main" role="main">
        <ErrorBoundary fullPage={ true }>
          <Switch>
            <Route exact path="/" component={ Search } />
            <Route exact path="/person/:personId" component={ Person } />
            <Route exact path="/end-session" component={ EndSession } />
            <Route exact path="/logout-success" component={ LogoutSuccess } />
            <Route path="*" component={ NotFound } />
          </Switch>
        </ErrorBoundary>
      </main>
      <IdleMonitor />
    </div>
  )
})

// Attach our Baobab store to the app for use in pages/components
const RootedApplication = root(initialState, Application)

export default RootedApplication
