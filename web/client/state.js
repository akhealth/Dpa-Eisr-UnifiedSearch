/* global ENVIRONMENT, API_URL */

import Baobab from 'baobab'

// Override API_URL for deployments behind WebSEAL
const getApiUrl = API_URL => {
  if (ENVIRONMENT === 'development') {
    if (window.location.host === 'uat.myaries.alaska.gov') {
      return 'https://uat.myaries.alaska.gov/search-api-dev'
    }
    if (window.location.host === 'stage.myaries.alaska.gov') {
      return 'https://stage.myaries.alaska.gov/search-api-dev'
    }
    if (window.location.host === 'dev.myaries.alaska.gov') {
      return 'https://dev.myaries.alaska.gov/search-api-dev'
    }
  }
  return API_URL
}

// Grab the basename for the deployed application. This ensures the
// site works when hosted behind WebSEAL junctions/routes.
const getBaseName = () => {
  if (ENVIRONMENT === 'development') {
    if (window.location.host.match(/myaries.alaska.gov/)) {
      return '/search-web-dev'
    }
  }
  return '/'
}

const initialState = new Baobab({
  config: {
    // Globally defined at application build time.
    // See the webpack config for more information.
    apiUrl: getApiUrl(API_URL),
    environment: ENVIRONMENT,

    basename: getBaseName(),

    // Session/inactivity timeout in seconds
    timeout: 15 * 60,

    // Request timeout in milliseconds
    requestTimeout: 100 * 1000,

    // Time between webseal session refreshes in seconds
    sessionRefresh: 5 * 60,

    // If this flag is true then our custom error handler will not log errors
    // to the console. This is used for tests
    suppressLogging: false
  },

  // history object provided by react-router-dom
  history: null,

  // Show or hide the header; useful if site is embedded in another page
  showHeader: true,

  // Information about the current user
  user: {
    username: null
  },

  searchPage: {
    isFirstSearch: true,
    searchResults: [],
    searchRunning: false,
    tooManyResults: false,
    timedOut: false,
    requestError: false,
    activeSearchQuery: null,
    validation: {
      errors: {
        ssn: false,
        firstName: false,
        lastName: false,
        ariesClientId: false,
        eisClientId: false
      },
      showErrors: {
        ssn: false,
        ariesClientId: false,
        eisClientId: false,
        firstName: false,
        lastName: false
      }
    },

    enabledFields: {
      ssnInputEnabled: '',
      eisClientIdInputEnabled: '',
      ariesClientIdInputEnabled: ''
    },

    formValues: {
      ariesClientId: '',
      eisClientId: '',
      ssn: '',
      lastName: '',
      firstName: ''
    }
  },

  idleMonitor: {
    counter: 0,
    enabled: true,
    running: false,
    timer: null,
    showDebug: false,
    sinceRefresh: 0,
    refreshPending: false
  },

  personDetailsPage: {
    personDetailsData: null,
    timedOut: false,
    requestError: false
  },

  endSessionTimeout: null
})

export default initialState
