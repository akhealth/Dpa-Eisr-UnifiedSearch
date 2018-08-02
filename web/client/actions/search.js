import superagent from 'superagent'
import isEqual from 'lodash.isequal'

import state from '-/state'
import logError from './error-handler'

const apiUrl = state.get('config', 'apiUrl')

const searchPage = state.select('searchPage')

export const performSearch = async (firstName, lastName, ssn) => {
  searchPage.set('searchRunning', true)
  searchPage.set('tooManyResults', false)
  searchPage.set('isFirstSearch', false)
  searchPage.set('searchResults', [])
  searchPage.set('timedOut', false)
  searchPage.set('requestError', false)

  try {
    const query = {
      firstName,
      lastName,
      registration: ssn
    }
    searchPage.set('activeSearchQuery', query)
    const reqTimeout = state.get('config', 'requestTimeout')

    const result = await superagent.get(`${apiUrl}/search/getmci`).query(query).timeout({ deadline: reqTimeout })
    let mciData = null
    let tooManyResults = false
    if (
      result.body.success === true &&
      isEqual(searchPage.get('activeSearchQuery'), query)
    ) {
      if (result.body.data != null) {
        tooManyResults = result.body.data.length > 50
        if (tooManyResults) {
          mciData = result.body.data.slice(0, 50)
        } else {
          mciData = result.body.data
        }
        if (ssn !== '' && mciData.length === 1) {
          const ssns = mciData[0].registrations.registration
            .filter(r => r.registrationName === 'SSN')
            .map(r => r.registrationValue)

          const history = state.get('history')
          if (history) {
            history.push(`/person/${ssns[0]}`)
          }

          // This is used for testing that the jump works
          searchPage.set('jumpToPerson', ssns[0])
        }
      }

      searchPage.set('searchResults', mciData)
      searchPage.set('tooManyResults', tooManyResults)
      searchPage.set('searchRunning', false)
    } else if (result.body.success !== true) {
      searchPage.set('requestError', true)
    }
  } catch (error) {
    if (error.errno === 'ETIME') {
      searchPage.set('timedOut', true)
    } else if (error && error.response && error.response.body && error.response.body.data &&
      error.response.body.data.error && error.response.body.data.error.match(/The operation has timed out/)
    ) {
      searchPage.set('timedOut', true)
    } else {
      searchPage.set('requestError', true)
    }

    logError('Error occurred in performSearch method of search.js', error)
    searchPage.set('searchRunning', false)
  }
}

export const resetSearchCount = () => {
  searchPage.set('isFirstSearch', true)
}

export const showValidationError = (key, showError) => {
  searchPage.set([ 'validation', 'showErrors', key ], showError)
}

export const setValidationError = (key, error) => {
  searchPage.set([ 'validation', 'errors', key ], error)
}

export const clearSearch = () => {
  resetSearchCount()
  searchPage.set('searchResults', [])
  searchPage.set('activeSearchQuery', {})
  searchPage.set('searchRunning', false)
  searchPage.set('requestError', false)
  searchPage.set('timedOut', false)
  searchPage.set('tooManyResults', false)

  // Reset all enabledFields in app state
  Object.keys(searchPage.get('enabledFields')).map(k => {
    searchPage.set([ 'enabledFields', k ], '')
  })

  // Reset all form values in app state
  Object.keys(searchPage.get('formValues')).map(k => {
    searchPage.set([ 'formValues', k ], '')
  })

  // Reset all validation errors in app state
  Object.keys(searchPage.get([ 'validation', 'errors' ])).map(k => {
    showValidationError(k, false)
  })

  // Reset all show error flags in app state
  Object.keys(searchPage.get([ 'validation', 'showErrors' ])).map(k => {
    showValidationError(k, false)
  })
}

export const ssnInputEntered = value => {
  searchPage.set([ 'formValues', 'ssn' ], value)

  if (value) {
    searchPage.set([ 'enabledFields', 'eisClientIdInputEnabled' ], 'disabled')
    searchPage.set([ 'validation', 'showErrors', 'eisClientId' ], false)
    searchPage.set([ 'enabledFields', 'ariesClientIdInputEnabled' ], 'disabled')
    searchPage.set([ 'validation', 'showErrors', 'ariesClientId' ], false)
    searchPage.tree.commit()
    return
  }

  searchPage.set([ 'enabledFields', 'eisClientIdInputEnabled' ], '')
  if (searchPage.get('validation', 'errors', 'eisClientId')) {
    searchPage.set([ 'validation', 'showErrors', 'eisClientId' ], true)
  }
  searchPage.set([ 'enabledFields', 'ariesClientIdInputEnabled' ], '')
  if (searchPage.get('validation', 'errors', 'ariesClientId')) {
    searchPage.set([ 'validation', 'showErrors', 'ariesClientId' ], true)
  }
  searchPage.tree.commit()
}

export const ariesInputEntered = value => {
  searchPage.set([ 'formValues', 'ariesClientId' ], value)

  if (value) {
    searchPage.set([ 'enabledFields', 'eisClientIdInputEnabled' ], 'disabled')
    searchPage.set([ 'validation', 'showErrors', 'eisClientId' ], false)
    searchPage.set([ 'enabledFields', 'ssnInputEnabled' ], 'disabled')
    searchPage.set([ 'validation', 'showErrors', 'ssn' ], false)
    searchPage.tree.commit()
    return
  }

  searchPage.set([ 'enabledFields', 'eisClientIdInputEnabled' ], '')
  if (searchPage.get('validation', 'errors', 'eisClientId')) {
    searchPage.set([ 'validation', 'showErrors', 'eisClientId' ], true)
  }
  searchPage.set([ 'enabledFields', 'ssnInputEnabled' ], '')
  if (searchPage.get('validation', 'errors', 'ssn')) {
    searchPage.set([ 'validation', 'showErrors', 'ssn' ], true)
  }
  searchPage.tree.commit()
}

export const eisInputEntered = value => {
  searchPage.set([ 'formValues', 'eisClientId' ], value)

  if (value) {
    searchPage.set([ 'enabledFields', 'ssnInputEnabled' ], 'disabled')
    searchPage.set([ 'validation', 'showErrors', 'ssn' ], false)
    searchPage.set([ 'enabledFields', 'ariesClientIdInputEnabled' ], 'disabled')
    searchPage.set([ 'validation', 'showErrors', 'ariesClientId' ], false)
    searchPage.tree.commit()
    return
  }

  searchPage.set([ 'enabledFields', 'ssnInputEnabled' ], '')
  if (searchPage.get('validation', 'errors', 'ssn')) {
    searchPage.set([ 'validation', 'showErrors', 'ssn' ], true)
  }
  searchPage.set([ 'enabledFields', 'ariesClientIdInputEnabled' ], '')
  if (searchPage.get('validation', 'errors', 'ariesClientId')) {
    searchPage.set([ 'validation', 'showErrors', 'ariesClientId' ], true)
  }
  searchPage.tree.commit()
}

export const lastNameInputEntered = value => {
  searchPage.set([ 'formValues', 'lastName' ], value)
  searchPage.tree.commit()
}

export const firstNameInputEntered = value => {
  searchPage.set([ 'formValues', 'firstName' ], value)
  searchPage.tree.commit()
}
