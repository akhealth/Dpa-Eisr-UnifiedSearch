import React from 'react'
import PropTypes from 'prop-types'
import FontAwesomeIcon from '@fortawesome/react-fontawesome'
import faExclamationTriangle from '@fortawesome/fontawesome-free-solid/faExclamationTriangle'

import Page from '-/components/Page/Page'
import logError from '-/actions/error-handler'
import './ErrorBoundary.scss'

export const ErrorPage = ({ fullPage = 0, errorMessage }) => {
  if (fullPage) {
    return (
      <Page>
        <h2>
          <FontAwesomeIcon icon={ faExclamationTriangle } className="error-exclamation-icon" />
          <span className="error-message">{errorMessage || 'Failed to load page'}</span>
        </h2>
      </Page>
    )
  } else {
    return (
      <div className='error-div'>
        <FontAwesomeIcon icon={ faExclamationTriangle } className="error-exclamation-icon" />
        <span className="error-message">{errorMessage || 'Failed to load'}</span>
      </div >
    )
  }
}

export class ErrorBoundary extends React.Component {
  constructor (props) {
    super(props)
    this.state = { hasError: false }
  }

  componentDidCatch (error, info) {
    this.setState({ hasError: true })

    logError('Error occurred in component', error, info, false)
  }

  render () {
    if (this.state.hasError) {
      return <ErrorPage fullPage={ this.props.fullPage } errorMessage={ this.props.errorMessage } />
    }
    return this.props.children
  }
}
ErrorBoundary.propTypes = {
  children: PropTypes.node,
  errorMessage: PropTypes.string,
  fullPage: PropTypes.bool
}

export default ErrorBoundary
