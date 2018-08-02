import React from 'react'
import { branch } from 'baobab-react/higher-order'

import {
  startCounter,
  incrementCounter,
  resetCounter,
  setTimer
} from '-/actions/idle-monitor'
import './IdleMonitor.scss'

export const IdleMonitor = ({
  counter,
  enabled,
  running,
  timer,
  showDebug,
  sinceRefresh,
  timeout
}) => {
  // If monitoring is disabled return immediately
  if (!enabled) {
    return false
  }

  // If the window timer doesn't exist yet, create it
  // and any needed event listeners to detect user activity
  if (!timer) {
    startCounter()

    // Reset the counter after any detectable events
    document.onmousemove = () => resetCounter()
    document.onkeydown = () => resetCounter()
    document.onclick = () => resetCounter()

    // Save the 1-second window timer in the app state
    const timer = window.setInterval(incrementCounter, 1000)
    setTimer(timer)
  }

  // Display a simple debug interface for testing purposes
  if (showDebug) {
    return (
      <div className="idle-monitor-debug">
        <label>Idle Monitor</label>
        <hr />
        <label>Running: </label>
        <span>{'' + running} </span>
        <br />
        <label>Idle Time: </label>
        <span>{counter} seconds </span>
        <br />
        <label>Timeout: </label>
        <span>{timeout} seconds </span>
        <br />
        <label>Since Last Ping: </label>
        <span>{sinceRefresh} seconds </span>
      </div>
    )
  }

  return true
}

export default branch(
  {
    counter: [ 'idleMonitor', 'counter' ],
    enabled: [ 'idleMonitor', 'enabled' ],
    running: [ 'idleMonitor', 'running' ],
    timer: [ 'idleMonitor', 'timer' ],
    showDebug: [ 'idleMonitor', 'showDebug' ],
    sinceRefresh: [ 'idleMonitor', 'sinceRefresh' ],
    timeout: [ 'config', 'timeout' ]
  },
  IdleMonitor
)
