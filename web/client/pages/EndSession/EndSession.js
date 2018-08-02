import React from 'react'

import { endSession, setSessionTimeout } from '-/actions/session'
import Page from '-/components/Page/Page'
import './EndSession.scss'

const EndSession = () => {
  // End the session and sign the user out
  const timeout = window.setTimeout(endSession, 3000)
  setSessionTimeout(timeout)

  return (
    <Page className="end-session-page">
      <h2>Logging you out...</h2>
    </Page>
  )
}

export default EndSession
