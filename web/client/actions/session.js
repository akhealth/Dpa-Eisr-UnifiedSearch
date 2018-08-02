import superagent from 'superagent'
import state from '-/state'

const apiUrl = state.get('config', 'apiUrl')

export const setHistory = history => {
  state.set('history', history)
}

export const setSessionTimeout = timeout => {
  state.set('endSessionTimeout', timeout)
}

export const endSession = async () => {
  try {
    await superagent.get(`${apiUrl}/user/end-session`)
  } catch (err) {
    // Do nothing since the user's logout should be properly
    // logged after they redirect to the standard ARIES logout
  }

  // Redirect to standard ARIES logout when deployed behind WebSeal. This
  // will create a proper audit trail in ARIES and redirect to the sign-on page.
  if (window.location.host.match(/myaries.alaska.gov/)) {
    return window.location.replace(
      `${window.location.origin}/pkmslogout?redirectWP=YES`
    )
  }

  state.get('history').push('/logout-success')
}
