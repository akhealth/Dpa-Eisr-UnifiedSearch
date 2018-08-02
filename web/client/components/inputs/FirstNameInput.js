import React from 'react'
import PropTypes from 'prop-types'

// First Name search form input
export const FirstNameInput = ({
  maxLength,
  showError,
  error,
  setError,
  toggleError,
  clearMissingLastNameError,
  onInput,
  value
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
    if (e.target.value === '') {
      clearMissingLastNameError()
    }
  }

  return (
    <div className="first-name control-group">
      <label htmlFor="search-firstName">First Name</label>
      <input
        id="search-firstName"
        name="searchFirstName"
        type="text"
        title="First Name"
        maxLength={ maxLength }
        style={ { border: showError ? '1px solid red' : '' } }
        onChange={ e => handleChange(e) }
        value={ value }
      />
      {showError ? <span className="error">{error}</span> : false}
    </div>
  )
}

FirstNameInput.propTypes = {
  maxLength: PropTypes.oneOfType([ PropTypes.number, PropTypes.string ]),
  error: PropTypes.oneOfType([ PropTypes.bool, PropTypes.string ]),
  showError: PropTypes.bool,
  setError: PropTypes.func,
  toggleError: PropTypes.func,
  clearMissingLastNameError: PropTypes.func,
  onInput: PropTypes.func,
  value: PropTypes.string
}

export default FirstNameInput
