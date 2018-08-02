import React from 'react'
import PropTypes from 'prop-types'
import FontAwesomeIcon from '@fortawesome/react-fontawesome'
import faExclamationCircle from '@fortawesome/fontawesome-free-solid/faExclamationCircle'

// Last Name search form input
export const LastNameInput = ({
  maxLength,
  showError,
  error,
  setError,
  toggleError,
  value,
  onInput
}) => {
  const validate = value => {
    if (value.length >= maxLength) {
      setError(`Must be less than ${maxLength} characters`)
      toggleError(true)
    } else {
      setError(false)
      toggleError(false)
    }
  }

  const handleChange = e => {
    onInput(e.target.value)
    validate(e.target.value)
  }

  return (
    <div className="last-name control-group">
      <label htmlFor="search-lastName">Last Name</label>
      <input
        id="search-lastName"
        name="searchLastName"
        type="text"
        title="Last Name"
        maxLength={ maxLength }
        style={ { border: showError ? '1px solid red' : '' } }
        onChange={ e => handleChange(e) }
        value={ value }
      />
      {showError ? (
        <div>
          {error === 'Last name is required with first name' ? <FontAwesomeIcon
            style={ {
              float: 'right',
              transform: 'translate(-10px, -30px)',
              color: 'red'
            } }
            icon={ faExclamationCircle }
          /> : false}
          <span className="error">{error}</span>
        </div>
      ) : (
        false
      )}
    </div>
  )
}

LastNameInput.propTypes = {
  maxLength: PropTypes.oneOfType([ PropTypes.number, PropTypes.string ]),
  showError: PropTypes.bool,
  error: PropTypes.oneOfType([ PropTypes.bool, PropTypes.string ]),
  toggleError: PropTypes.func,
  setError: PropTypes.func,
  value: PropTypes.string,
  onInput: PropTypes.func
}

export default LastNameInput
