import React from 'react'
import PropTypes from 'prop-types'
import FontAwesomeIcon from '@fortawesome/react-fontawesome'
import faExclamationCircle from '@fortawesome/fontawesome-free-solid/faExclamationCircle'

export const EisClientIdInput = ({
  error,
  showError,
  enabled,
  setError,
  toggleError,
  onInput,
  value
}) => {
  // EIS Client IDs must start with 06 and be 10 digits long
  const validate = ({ target }) => {
    const validEisFormat = /06[0-9]{8}/
    if (target.value !== '' && !validEisFormat.test(target.value)) {
      setError('Invalid EIS Client ID')
      toggleError(true)
    } else {
      setError(false)
      toggleError(false)
    }
  }

  const handleChange = e => {
    // Disable other fields and commit state
    onInput(e.target.value)
  }
  return (
    <div className="eis control-group">
      <label htmlFor="search-eisId">EIS Client ID</label>
      <input
        id="search-eisId"
        name="searchEisId"
        type="text"
        title="EIS Client ID"
        disabled={ enabled }
        maxLength="10"
        style={ { border: showError ? '1px solid red' : '' } }
        onChange={ e => handleChange(e) }
        onBlur={ validate }
        value={ value }
      />
      {showError ? (
        <div>
          <FontAwesomeIcon
            style={ {
              float: 'right',
              transform: 'translate(-10px, -30px)',
              color: 'red'
            } }
            icon={ faExclamationCircle }
          />
          <span className="error">{error}</span>
        </div>
      ) : (
        false
      )}
    </div>
  )
}

EisClientIdInput.propTypes = {
  showError: PropTypes.bool,
  error: PropTypes.oneOfType([ PropTypes.bool, PropTypes.string ]),
  enabled: PropTypes.string,
  setError: PropTypes.func,
  toggleError: PropTypes.func,
  onInput: PropTypes.func,
  value: PropTypes.string
}

export default EisClientIdInput
