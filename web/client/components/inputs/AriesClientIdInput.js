import React from 'react'
import PropTypes from 'prop-types'
import FontAwesomeIcon from '@fortawesome/react-fontawesome'
import faExclamationCircle from '@fortawesome/fontawesome-free-solid/faExclamationCircle'

export const AriesClientIdInput = ({
  error,
  showError,
  enabled,
  setError,
  toggleError,
  onInput,
  value
}) => {
  // Aries Client IDs must start with 2 and be 10 digits long
  const validate = ({ target }) => {
    const validAriesFormat = /2[0-9]{9}/
    if (target.value !== '' && !validAriesFormat.test(target.value)) {
      setError('Invalid ARIES Client ID')
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
    <div className="aries control-group">
      <label htmlFor="search-ariesId">ARIES Client ID</label>
      <input
        id="search-ariesId"
        name="searchAriesId"
        type="text"
        title="ARIES Client ID"
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

AriesClientIdInput.propTypes = {
  showError: PropTypes.bool,
  error: PropTypes.oneOfType([ PropTypes.bool, PropTypes.string ]),
  enabled: PropTypes.string,
  setError: PropTypes.func,
  toggleError: PropTypes.func,
  onInput: PropTypes.func,
  value: PropTypes.string
}

export default AriesClientIdInput
