import superagent from 'superagent'
import state from '-/state'
import logError from './error-handler'

const apiUrl = state.get('config', 'apiUrl')

const personPage = state.select('personDetailsPage')

export const getPersonDetails = async personId => {
  try {
    personPage.set('timedOut', false)
    personPage.set('requestError', false)

    const reqTimeout = state.get('config', 'requestTimeout')
    const result = await superagent
      .get(`${apiUrl}/person/${personId}`)
      .timeout({ deadline: reqTimeout })

    if (result.body.success) {
      const personDetailsData = result.body.data

      personPage.set('personDetailsData', personDetailsData)
    } else {
      personPage.set('requestError', true)
    }
  } catch (error) {
    if (error.errno === 'ETIME') {
      personPage.set('timedOut', true)
    } else if (
      error.response &&
      error.response.body &&
      error.response.body.data &&
      error.response.body.data.error &&
      error.response.body.data.error.match(/The operation has timed out/)
    ) {
      personPage.set('timedOut', true)
    } else {
      personPage.set('requestError', true)
    }

    logError('Error occurred in getPersonDetails method of person.js', error)
  }
}

export const clearPersonDetails = () => {
  personPage.set('personDetailsData', null)
  personPage.set('timedOut', false)
  personPage.set('requestError', false)
}
