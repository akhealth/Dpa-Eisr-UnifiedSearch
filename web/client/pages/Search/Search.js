import React from 'react'
import { branch } from 'baobab-react/higher-order'
import PropTypes from 'prop-types'

import {
  performSearch,
  showValidationError,
  setValidationError,
  clearSearch,
  ssnInputEntered,
  ariesInputEntered,
  eisInputEntered,
  firstNameInputEntered,
  lastNameInputEntered
} from '-/actions/search'
import { clearPersonDetails } from '-/actions/person'
import Page from '-/components/Page/Page'
import SearchResults from '-/components/SearchResults/SearchResults'
import FirstNameInput from '-/components/inputs/FirstNameInput'
import LastNameInput from '-/components/inputs/LastNameInput'
import AriesClientIdInput from '-/components/inputs/AriesClientIdInput'
import EisClientIdInput from '-/components/inputs/EisClientIdInput'
import SsnInput from '-/components/inputs/SsnInput'
import ErrorBoundary from '-/components/ErrorBoundary/ErrorBoundary'
import './Search.scss'

// Handle the form submission and perform a search using the API
export const handleSubmit = (event, displayedErrors, enabledFields) => {
  event.preventDefault()

  // Remove focus from the inputs - this helps clear the
  // autofill/autocomplete popups afters a user performs a search
  Array.from(document.querySelectorAll('input')).forEach(input => {
    input.blur()
  })

  // Verify that there is a last name if there is a first name
  if (event.target.searchFirstName.value && !event.target.searchLastName.value) {
    setValidationError('lastName', 'Last name is required with first name')
    showValidationError('lastName', true)
    return false
  }

  // Verify we don't have any validation errors before searching
  if (Object.values(displayedErrors).some(v => v)) {
    return false
  }

  const firstName = event.target.searchFirstName.value
  const lastName = event.target.searchLastName.value
  let registration = ''

  // Choose the active registration field
  const ssn = event.target.searchSSN.value.replace(/-/g, '') // Strip hyphens
  const ariesClientId = event.target.searchAriesId.value
  const eisClientId = event.target.searchEisId.value

  // Verify there is some search criteria
  const searchInputs = [ firstName, lastName, ssn, ariesClientId, eisClientId ]
  if (searchInputs.every(p => !p)) {
    return false
  }

  // Only one can be valid at a time
  // Registration fields do not validate when user hits enter without focusing out, so validate here as well
  if (enabledFields.ssnInputEnabled === '' && ssn !== '') {
    const ssnRegex = /^(?!666|000|9\d{2})\d{3}(-?)(?!00)\d{2}\1(?!0{4})\d{4}$/
    if (ssnRegex.test(ssn)) {
      registration = ssn
    } else {
      return false
    }
  } else if (
    enabledFields.ariesClientIdInputEnabled === '' &&
    ariesClientId !== ''
  ) {
    const ariesRegex = /2[0-9]{9}/
    if (ariesRegex.test(ariesClientId)) {
      registration = ariesClientId
    } else {
      return false
    }
  } else if (
    enabledFields.eisClientIdInputEnabled === '' &&
    eisClientId !== ''
  ) {
    const eisRegex = /06[0-9]{8}/
    if (eisRegex.test(eisClientId)) {
      registration = eisClientId
    } else {
      return false
    }
  }

  performSearch(firstName, lastName, registration)
}

const SearchForm = ({ validation, enabledFields, formValues }) => {
  return (
    <form
      onSubmit={ event =>
        handleSubmit(event, validation.showErrors, enabledFields)
      }
    >

      <AriesClientIdInput
        error={ validation.errors.ariesClientId }
        showError={ validation.showErrors.ariesClientId }
        enabled={ enabledFields.ariesClientIdInputEnabled }
        setError={ error => setValidationError('ariesClientId', error) }
        toggleError={ showError => showValidationError('ariesClientId', showError)
        }
        onInput={ ariesInputEntered }
        value={ formValues.ariesClientId }
      />
      <EisClientIdInput
        error={ validation.errors.eisClientId }
        showError={ validation.showErrors.eisClientId }
        enabled={ enabledFields.eisClientIdInputEnabled }
        setError={ error => setValidationError('eisClientId', error) }
        toggleError={ showError => showValidationError('eisClientId', showError) }
        onInput={ eisInputEntered }
        value={ formValues.eisClientId }
      />
      <SsnInput
        error={ validation.errors.ssn }
        showError={ validation.showErrors.ssn }
        enabled={ enabledFields.ssnInputEnabled }
        setError={ error => setValidationError('ssn', error) }
        toggleError={ showError => showValidationError('ssn', showError) }
        onInput={ ssnInputEntered }
        value={ formValues.ssn }
      />
      <LastNameInput
        maxLength="40"
        showError={ validation.showErrors.lastName }
        error={ validation.errors.lastName }
        setError={ error => setValidationError('lastName', error) }
        toggleError={ showError => showValidationError('lastName', showError) }
        onInput={ lastNameInputEntered }
        value={ formValues.lastName }
      />
      <FirstNameInput
        maxLength="40"
        showError={ validation.showErrors.firstName }
        error={ validation.errors.firstName }
        setError={ error => setValidationError('firstName', error) }
        toggleError={ showError => showValidationError('firstName', showError) }
        clearMissingLastNameError={ () => showValidationError('lastName', false) }
        onInput={ firstNameInputEntered }
        value={ formValues.firstName }
      />

      <div className="search-buttons">
        <button id="cancel-search" type="reset" onClick={ () => clearSearch() }>
          Clear
        </button>
        <button type="submit">Search</button>
      </div>
    </form>
  )
}

SearchForm.propTypes = {
  validation: PropTypes.object,
  enabledFields: PropTypes.object,
  formValues: PropTypes.object
}

// Main Search Page component
export const Search = ({
  searchResults,
  searchRunning,
  isFirstSearch,
  tooManyResults,
  validation,
  enabledFields,
  formValues,
  timedOut,
  requestError
}) => {
  // Clear the active person from any previous searches
  clearPersonDetails()

  return (
    <Page className="search-page">
      <div className="search-form">
        <ErrorBoundary errorMessage="Failed to load search form">
          <SearchForm
            validation={ validation }
            enabledFields={ enabledFields }
            formValues={ formValues }
          />
        </ErrorBoundary>
      </div>

      <div className="search-results-wrapper">
        <ErrorBoundary errorMessage="Failed to load search results">
          <SearchResults
            isFirstSearch={ isFirstSearch }
            searchRunning={ searchRunning }
            searchResults={ searchResults }
            timedOut={ timedOut }
            requestError={ requestError }
            tooManyResults={ tooManyResults }
          />
        </ErrorBoundary>
      </div>
    </Page>
  )
}

Search.propTypes = {
  searchResults: PropTypes.array,
  searchRunning: PropTypes.bool,
  tooManyResults: PropTypes.bool,
  isFirstSearch: PropTypes.bool,
  validation: PropTypes.object,
  jumpToPerson: PropTypes.string,
  enabledFields: PropTypes.object,
  formValues: PropTypes.object,
  timedOut: PropTypes.bool,
  requestError: PropTypes.bool
}

export default branch(
  {
    isFirstSearch: [ 'searchPage', 'isFirstSearch' ],
    searchResults: [ 'searchPage', 'searchResults' ],
    searchRunning: [ 'searchPage', 'searchRunning' ],
    tooManyResults: [ 'searchPage', 'tooManyResults' ],
    validation: [ 'searchPage', 'validation' ],
    enabledFields: [ 'searchPage', 'enabledFields' ],
    formValues: [ 'searchPage', 'formValues' ],
    timedOut: [ 'searchPage', 'timedOut' ],
    requestError: [ 'searchPage', 'requestError' ]
  },
  Search
)
