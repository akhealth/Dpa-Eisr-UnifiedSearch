import React from 'react'
import { branch } from 'baobab-react/higher-order'
import PropTypes from 'prop-types'
import { Link } from 'react-router-dom'
import FontAwesomeIcon from '@fortawesome/react-fontawesome'
import faChevronCircleLeft from '@fortawesome/fontawesome-free-solid/faChevronCircleLeft'
import faSpinner from '@fortawesome/fontawesome-free-solid/faSpinner'
import faExclamationTriangle from '@fortawesome/fontawesome-free-solid/faExclamationTriangle'

import { getPersonDetails } from '-/actions/person'
import Page from '-/components/Page/Page'
import './Person.scss'
import { ErrorBoundary, ErrorPage } from '-/components/ErrorBoundary/ErrorBoundary'

export const CasePrograms = ({ programs, personalInfo }) => {
  if (!programs || programs.length === 0) {
    return (
      <div className="case-no-programs">
        No involvement for {personalInfo.firstName}
      </div>
    )
  }

  return programs.map((program, i) => {
    let statusClass = 'case-value'
    if (
      program.programStatus.match(
        /^(Approved|Pending|Open|Received|Suspended)$/
      )
    ) {
      statusClass = 'blue-status'
    } else if (program.programStatus.match(/^(Denied|Closed)$/)) {
      statusClass = 'gray-status'
    }

    return (
      <div key={ i } className="case-program-container">
        <span className="program-name">{program.programName}</span>
        <span className="program-info">
          {program.programName === 'AP' || program.programName === 'ME' ? (
            <div className="program-value-container">
              <span className="case-label"> Category: </span>
              <span className="case-value">{program.programSubtype}</span>
            </div>
          ) : null}
          {program.eligibilityCode && program.eligibilityCode !== '' ? (
            <div className="program-value-container">
              <span className="case-label"> Eligibility Code: </span>
              <span className="case-value program-eligibility">
                {program.eligibilityCode}
              </span>
            </div>
          ) : null}
          {program.programName === 'ME' &&
          program.medicaidSubType &&
          program.medicaidSubType !== '' ? (
              <div className="program-value-container">
                <span className="case-label"> Medicaid Subtype: </span>
                <span className="case-value program-subType">
                  {program.medicaidSubType}
                </span>
              </div>
            ) : null}
          <div className="program-value-container">
            <span className="case-label"> Status: </span>
            <span className={ statusClass }>{program.programStatus}</span>
          </div>
        </span>
        <span className="issuance-info">
          <div className="program-value-container">
            <span className="case-label"> Issuance date: </span>
            <span className="case-value">
              {program.issuances.length === 0
                ? null
                : program.issuances[0].issuanceDateString}
            </span>
          </div>
          {program.programName !== 'ME' ? (
            <div className="program-value-container">
              <span className="case-label"> Benefit: </span>
              <span className="case-value">
                {program.issuances.length === 0
                  ? null
                  : program.issuances[0].issuanceAmount}
              </span>
            </div>
          ) : null}
          {program.issuances.length !== 0 &&
          program.issuances[0].issuanceType !== null ? (
              <div className="program-value-container">
                <span className="case-label"> Type: </span>
                <span className="case-value program-type">
                  {program.issuances[0].issuanceType}
                </span>
              </div>
            ) : null}
          <div className="certification-date">
            <span className="case-label"> Certification date: </span>
            <span className="case-value">
              {program.certificationStart + ' - ' + program.certificationEnd}
            </span>
          </div>
        </span>
      </div>
    )
  })
}

export const CaseSummary = ({ caseInfo, personalInfo }) => {
  return (
    <div className="case-summary">
      <span className="case-overview">
        <div className="case-value-container">
          <span className="case-label"> Source </span>
          <span className="case-value">{caseInfo.location}</span>
        </div>
        <div className="case-value-container">
          <span className="case-label"> Case no. </span>
          <span className="case-value">
            {caseInfo.location === 'ARIES' ? (
              <a
                href={ `/wp/ControllerServlet?PAGE_ID=IQCSS&ACTION=ClickOnLink&caseOrApplicationNumber=${
                  caseInfo.caseNumber
                }&token=Random` }
              >
                {caseInfo.caseNumber}
              </a>
            ) : (
              caseInfo.caseNumber
            )}
          </span>
        </div>
        <div className="case-value-container">
          <span className="case-label"> PI </span>
          <span className="case-value">
            {caseInfo.primaryIndividual.firstName +
              ' ' +
              caseInfo.primaryIndividual.lastName}
            <br />
            ({caseInfo.primaryIndividual.clientId})
          </span>
        </div>
      </span>

      <span className="case-details">
        <div className="case-info">
          <div className="case-detail-header">
            {personalInfo.firstName + "'s involvement"}
          </div>
          <div className="case-clientId-container">
            <span className="case-label"> Client ID used: </span>
            <span className="case-value">{caseInfo.clientId}</span>
          </div>
        </div>

        <div className="case-programs">
          <ErrorBoundary errorMessage="Failed to load case programs">
            <CasePrograms
              programs={ caseInfo.programs }
              personalInfo={ personalInfo }
            />
          </ErrorBoundary>
        </div>
      </span>
    </div>
  )
}
CaseSummary.propTypes = {
  caseInfo: PropTypes.object.isRequired,
  personalInfo: PropTypes.object.isRequired
}

export const ApplicationRecord = ({ application }) => {
  let statusClass = 'app-value'
  if (
    application.status.match(
      /^(Application Complete|Application Received|Pending)$/
    )
  ) {
    statusClass = 'blue-value'
  } else if (
    application.status.match(
      /^(Application Approved|Application Denied|Application Withdrawn|Case Denied|Pending Application Denied|Denied)$/
    )
  ) {
    statusClass = 'gray-value'
  }

  return (
    <div className="application-summary">
      <span className="application-overview">
        <div className="app-value-container">
          <span className="app-label"> Source </span>
          <span className="app-value">{application.source}</span>
        </div>
        <div className="app-value-container">
          <span className="app-label"> Application no. </span>
          <span className="app-value">{application.applicationNumber}</span>
        </div>
      </span>
      <span className="application-details">
        <div className="app-value-container">
          <span className="app-label"> Received Date: </span>
          <span className="app-value">{application.receivedDateString}</span>
        </div>
        <div className="app-value-container">
          <span className="app-label"> Status: </span>
          <span className={ statusClass }>{application.status}</span>
        </div>
      </span>
    </div>
  )
}
ApplicationRecord.propTypes = {
  application: PropTypes.object.isRequired
}

export const ApplicationSummary = ({ applications }) => {
  if (!applications || applications.length === 0) {
    return <div className="no-applicatons">No registered applications.</div>
  }

  return applications.map((application, i) => (
    <ErrorBoundary key={ i } errorMessage="Failed to load application">
      <ApplicationRecord
        key={ i }
        application={ application }
      />
    </ErrorBoundary>
  ))
}

export const PersonHeader = ({ personalInfo, systemInfo }) => {
  const personSSNs = personalInfo.ssns.map(r => {
    if (r.length === 9) {
      r = r.replace(/^(\d{3})(\d{2})(\d{4})$/, '$1-$2-$3')
    }
    return r
  })

  return (
    <div className="person-header">
      <div className="personal-information">
        <div className="info-group person-name">
          <label> Name </label>
          <span>{personalInfo.name}</span>
        </div>
        <div className="info-group">
          <label> Social Security Number </label>
          <span>{personSSNs.join(', ')}</span>
        </div>
        <div className="info-group">
          <label> Date of Birth </label>
          <span>{personalInfo.dob}</span>
        </div>
      </div>

      <div className="program-information">
        <div className="info-group">
          <label> EIS Client ID </label>
          <span>{systemInfo.eisClientIds.join(', ')}</span>
        </div>
        <div className="info-group">
          <label> ARIES Client ID </label>
          <span>{systemInfo.ariesClientIds.join(', ')}</span>
        </div>
      </div>
    </div>
  )
}
PersonHeader.propTypes = {
  personalInfo: PropTypes.object.isRequired,
  systemInfo: PropTypes.object.isRequired
}

export const Person = ({ personDetailsData, timedOut, requestError, match }) => {
  if (!personDetailsData && !timedOut && !requestError) {
    getPersonDetails(match.params.personId)
    return (
      <Page className="person-page">
        <div id="loading-message">
          <FontAwesomeIcon
            style={ {
              marginRight: '5px'
            } }
            icon={ faSpinner }
            pulse
          />
          Fetching person details
        </div>
      </Page>
    )
  }

  if (requestError) {
    return <ErrorPage fullPage={ true } />
  }

  if (timedOut) {
    return (
      <Page className="person-page">
        <Link to="/" id="back-link">
          <FontAwesomeIcon icon={ faChevronCircleLeft } id="back-chevron" />
          Return to Search Results
        </Link>
        <div id="timed-out">
          <FontAwesomeIcon icon={ faExclamationTriangle } /> Your request has
          timed out. Please try again.
        </div>
      </Page>
    )
  }

  const caseSummaries = personDetailsData.cases.map((caseInfo, i) => (
    <ErrorBoundary key={ i } errorMessage="Failed to load case">
      <CaseSummary
        key={ i }
        caseInfo={ caseInfo }
        personalInfo={ personDetailsData.personalInfo }
      />
    </ErrorBoundary>
  ))

  return (
    <Page className="person-page">
      <Link to="/" id="back-link">
        <FontAwesomeIcon icon={ faChevronCircleLeft } id="back-chevron" />
        Return to Search Results
      </Link>
      <ErrorBoundary errorMessage="Failed to load person information">
        <PersonHeader
          personalInfo={ personDetailsData.personalInfo }
          systemInfo={ personDetailsData.systemInfo }
        />
      </ErrorBoundary>

      <h2>Applications</h2>
      <ErrorBoundary errorMessage="Failed to load applications">
        <ApplicationSummary applications={ personDetailsData.applications } />
      </ErrorBoundary>

      <h2>Cases</h2>
      {caseSummaries}
    </Page>
  )
}
Person.propTypes = {
  personDetailsData: PropTypes.object,
  timedOut: PropTypes.bool,
  requestError: PropTypes.bool,
  match: PropTypes.object
}

export default branch(
  {
    personDetailsData: [ 'personDetailsPage', 'personDetailsData' ],
    timedOut: [ 'personDetailsPage', 'timedOut' ],
    requestError: [ 'personDetailsPage', 'requestError' ]
  },
  Person
)
