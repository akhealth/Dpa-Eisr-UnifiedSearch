import React from 'react'
import PropTypes from 'prop-types'
import FontAwesomeIcon from '@fortawesome/react-fontawesome'
import faExclamationCircle from '@fortawesome/fontawesome-free-solid/faExclamationCircle'

const checkForAllSameDigits = input => {
  input = input.replace(/-/g, '') // Strip hyphens
  let sameCount = 0
  const firstDigit = input.charAt(0)
  for (let i = 1; i <= 8; i++) {
    if (input.charAt(i) === firstDigit) {
      sameCount++
    }
  }
  if (sameCount === 8) {
    return true // All digits are same
  }
  return false
}

// Social Security Number search form input
export const SsnInput = ({
  error,
  showError,
  enabled,
  setError,
  toggleError,
  onInput,
  value
}) => {
  /* Invalid SSN:
    Validate that 'SSN' must be 9 digit numeric.
    Validate that all digits are not the same.
    *SSN = AAA-BB-CCCC*
    > AAA cannot be '000' or '666'
    > AAA cannot be between 900-999
    > BB cannot be '00'
    > CCCC cannot be '0000'
  */
  const validate = ({ target }) => {
    const input = target.value
    const ssnRegex = /^(?!666|000|9\d{2})\d{3}(-?)(?!00)\d{2}\1(?!0{4})\d{4}$/
    if (input !== '') {
      if (ssnRegex.test(input)) {
        let allSameDigits = checkForAllSameDigits(input)
        if (allSameDigits) {
          // All digits are same
          setError('Invalid SSN format')
          toggleError(true)
        } else {
          // Valid
          setError(false)
          toggleError(false)
        }
      } else {
        setError('Invalid SSN format')
        toggleError(true)
      }
    } else {
      setError(false)
      toggleError(false)
    }
  }

  // Check for hyphens when pasting to set maxLength
  const handlePaste = e => {
    // Check for hyphens when pasting to set maxLength
    if (e.clipboardData.getData('Text').indexOf('-') >= 0 && e.target.value.indexOf('-')) {
      e.target.maxLength = 11
    } else {
      e.target.maxLength = 9
    }
  }

  const handleChange = e => {
    // Limit length dynamically
    if (e.target.value.indexOf('-') >= 0) {
      e.target.maxLength = 11
    } else {
      e.target.maxLength = 9
    }

    // Disable other fields and commit state
    onInput(e.target.value)
  }

  return (
    <div className="ssn control-group">
      <label htmlFor="search-SSN">Social Security Number</label>
      <input
        id="search-SSN"
        name="searchSSN"
        type="text"
        title="SSN"
        disabled={ enabled }
        maxLength="11"
        style={ { border: showError ? '1px solid red' : '' } }
        onBlur={ validate }
        onChange={ e => handleChange(e) }
        onPaste={ e => handlePaste(e) }
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

SsnInput.propTypes = {
  showError: PropTypes.bool,
  error: PropTypes.oneOfType([ PropTypes.bool, PropTypes.string ]),
  enabled: PropTypes.string,
  setError: PropTypes.func,
  toggleError: PropTypes.func,
  onInput: PropTypes.func,
  value: PropTypes.string
}

export default SsnInput
