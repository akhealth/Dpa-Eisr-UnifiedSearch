import superagent from 'superagent'

import state from '-/state'

const apiUrl = state.get('config', 'apiUrl')

// idle monitor cursor
const idleMonitor = state.select('idleMonitor')

export const startCounter = () => {
  idleMonitor.set('running', true)
}

export const stopCounter = () => {
  idleMonitor.set('running', false)
}

export const incrementCounter = () => {
  const counter = idleMonitor.select('counter')
  counter.set(counter.get() + 1)

  const maxTimeout = state.get([ 'config', 'timeout' ])
  if (counter.get() >= maxTimeout) {
    stopCounter()
    clearTimer()

    const history = state.get('history')
    if (history) {
      history.push('/end-session')
    }
  }

  const sinceRefresh = idleMonitor.select('sinceRefresh')
  sinceRefresh.set(sinceRefresh.get() + 1)

  if (idleMonitor.get('refreshPending')) {
    return
  }

  // Only refresh if more than the maxRefreshTime away from the web timeout,
  // otherwise we're refreshing past the web timeout
  const maxRefreshTime = state.get([ 'config', 'sessionRefresh' ])
  const sessionRefreshExceeded = sinceRefresh.get() >= maxRefreshTime
  const maxTimeoutExceeded = counter.get() > maxTimeout - maxRefreshTime

  if (sessionRefreshExceeded && !maxTimeoutExceeded) {
    refreshSession()
  }
}

export const resetCounter = () => {
  idleMonitor.set('counter', 0)
}

export const setTimer = timer => {
  idleMonitor.set('timer', timer)
}

export const clearTimer = () => {
  const timer = idleMonitor.get('timer')
  if (timer) {
    clearInterval(timer)
  }
}

export const refreshSession = async () => {
  try {
    // we only need one refresh request going at once
    idleMonitor.set('refreshPending', true)
    const reqTimeout = state.get('config', 'requestTimeout')

    const result = await superagent.get(`${apiUrl}/user/refresh`).timeout({ deadline: reqTimeout })

    if (result.body.success) {
      idleMonitor.set('sinceRefresh', 0)
      idleMonitor.set('refreshPending', false)
    }
  } catch (error) {
    idleMonitor.set('refreshPending', false)
    // TODO - Possibly display a timeout or error message, saying that api refresh has failed
  }
}
