import React from 'react'
import { Link } from 'react-router-dom'
import PropTypes from 'prop-types'
import FontAwesomeIcon from '@fortawesome/react-fontawesome'
import faExclamationTriangle from '@fortawesome/fontawesome-free-solid/faExclamationTriangle'
import faSpinner from '@fortawesome/fontawesome-free-solid/faSpinner'
import ErrorBoundary from '-/components/ErrorBoundary/ErrorBoundary'
import './SearchResults.scss'

const SearchResultsRow = ({ result }) => {
  const eisIds = result.registrations.registration
    .filter(r => r.registrationName === 'EIS_ID')
    .map(r => r.registrationValue)

  const eisIdsTableFields = eisIds.map(r => {
    return <p key={ r }>{r}</p>
  })

  const ariesIds = result.registrations.registration
    .filter(r => r.registrationName === 'ARIES_ID')
    .map(r => r.registrationValue)

  const ariesIdsTableFields = ariesIds.map(r => {
    return <p key={ r }>{r}</p>
  })

  const ssns = result.registrations.registration
    .filter(r => r.registrationName === 'SSN')
    .map(r => r.registrationValue)

  const ssnsTableFields = ssns.map(r => {
    if (r.length === 9) {
      r = r.replace(/^(\d{3})(\d{2})(\d{4})$/, '$1-$2-$3')
    }
    return <p key={ r }>{r}</p>
  })

  const registrationLink = ssns[0] || ariesIds[0] || eisIds[0]

  return (
    <tr>
      <td data-label="Last name">{result.lastName}</td>
      <td data-label="First, MI">
        {result.firstName}
        {result.middleName ? `, ${result.middleName[0]}` : ''}
      </td>
      <td data-label="SSN">
        {ssnsTableFields.length > 0 ? ssnsTableFields : '-'}
      </td>
      <td data-label="Birth">{result.formattedDateOfBirth}</td>
      <td data-label="EIS ID">
        {eisIdsTableFields.length > 0 ? eisIdsTableFields : '-'}
      </td>
      <td data-label="ARIES ID">
        {ariesIdsTableFields.length > 0 ? ariesIdsTableFields : '-'}
      </td>
      <td>
        {registrationLink ? (
          <Link
            aria-label={ `More information about ${
              result.firstName
            } ${result.middleName || ''} ${result.lastName}` }
            to={ `/person/${registrationLink}` }
          >
            more
          </Link>
        ) : (
          ''
        )}
      </td>
    </tr>
  )
}
SearchResultsRow.propTypes = {
  result: PropTypes.object.isRequired
}

export const SearchResults = ({
  isFirstSearch,
  searchResults,
  searchRunning,
  timedOut,
  requestError,
  tooManyResults
}) => {
  if (searchRunning) {
    return (
      <div className="search-results searching">
        <span>
          <FontAwesomeIcon
            style={ {
              marginRight: '5px'
            } }
            icon={ faSpinner }
            pulse
          />
          Searching for results
        </span>
      </div>
    )
  }

  const haveResults = searchResults && searchResults.length > 0

  if (isFirstSearch && !haveResults) {
    return (
      <div className={ 'search-results no-results' }>
        <span className="no-results">
          No results yet. Enter search criteria.
        </span>
      </div>
    )
  }

  if (requestError) {
    return (
      <div className={ 'search-results request-error' }>
        <span className='request-error'>
          <div id='request-error-message'><FontAwesomeIcon icon={ faExclamationTriangle } /> Failed to load search results.</div>
        </span>
      </div>
    )
  }

  if (timedOut) {
    return (
      <div className={ 'search-results timed-out' }>
        <span className='timed-out'>
          <div id='timed-out-message'><FontAwesomeIcon icon={ faExclamationTriangle } /> Your request has timed out. Please try again.</div>
        </span>
      </div>
    )
  }

  if (!haveResults) {
    return (
      <div className={ 'search-results no-results' }>
        <span className="no-results">
          <div id="no-results-message">No results found.</div>
          <div id="no-results-helper">Try different search criteria.</div>
        </span>
      </div>
    )
  }

  const rows = searchResults.map((result, i) => (
    <ErrorBoundary key={ i } errorMessage="Failed to load record">
      <SearchResultsRow key={ i } result={ result } />
    </ErrorBoundary>
  ))

  // Replace tab and new line characters with spaces, remove excess spaces
  // and trim leading + trailing whitespace
  const handleCopy = e => {
    e.preventDefault()
    let copiedText = window.getSelection().toString()
    let filteredText = copiedText.replace(/\t/g, ' ').replace(/\n/g, ' ').trim().replace(/ +(?= )/g, '')
    e.clipboardData.setData('Text', filteredText)
  }

  return (
    <div className="search-results">
      <div className="search-results-count">
        {tooManyResults ? (
          <span id="too-many-results">
            <FontAwesomeIcon icon={ faExclamationTriangle } /> Your search
            produced more than the 50 results shown here. Try refining your
            search.
          </span>
        ) : (
          <span>{searchResults.length} results found</span>
        )}
      </div>

      <table cellSpacing='0' cellPadding='0' onCopy={ e => handleCopy(e) }>
        <thead>
          <tr>
            <th>Last name</th>
            <th>First, MI</th>
            <th title="Social Security Number">SSN</th>
            <th>Birth</th>
            <th>EIS ID</th>
            <th>ARIES ID</th>
            <th />
          </tr>
        </thead>
        <tbody>{rows}</tbody>
      </table>
    </div>
  )
}
SearchResults.propTypes = {
  isFirstSearch: PropTypes.bool,
  searchResults: PropTypes.array,
  searchRunning: PropTypes.bool,
  timedOut: PropTypes.bool,
  requestError: PropTypes.bool,
  tooManyResults: PropTypes.bool
}

export default SearchResults
