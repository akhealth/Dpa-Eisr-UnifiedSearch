import state from '-/state'

const suppressLogging = state.select('config', 'suppressLogging')

export const logError = (message, errorDetails, extraDebugInfo, logInConsole = true) => {
  try {
    if (!logInConsole || suppressLogging.get() === true) {
      return
    }
    let errorInfo = {}
    if (message) {
      errorInfo.message = message
    }
    if (errorDetails.response) {
      errorInfo.responseInfo = errorDetails.response

      if (errorDetails.response.body) {
        errorInfo.debugResponseBody = errorDetails.response.body
      }
    }
    if (extraDebugInfo) {
      errorInfo.extraDebugInfo = extraDebugInfo
    }

    if (Object.keys(errorInfo).length) {
      console.error(errorDetails, errorInfo)
    } else {
      console.error(errorDetails)
    }
  } catch (e) {
    console.error(e)
  }
}

export default logError
