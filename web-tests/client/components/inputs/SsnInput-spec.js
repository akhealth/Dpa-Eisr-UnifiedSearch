/* global describe, it */

import '../../react-mocha-setup'

import React from 'react'
import { shallow } from 'enzyme'
import { expect } from 'chai'

import { SsnInput } from '-/components/inputs/SsnInput'

describe('SsnInput Component', () => {
  it('should store the value on change', () => {
    const enteredValue = '110101000'
    let error = false
    let showError = false
    let value = ''
    const setError = errorMessage => {
      error = errorMessage
    }
    const toggleError = show => {
      showError = show
    }
    const inputEntered = inputValue => {
      value = inputValue
    }
    const component = shallow(
      <SsnInput
        error="test error"
        setError={ setError }
        showError={ showError }
        toggleError={ toggleError }
        onInput={ inputEntered }
      />
    )
    component.find('input').simulate('change', {
      target: {
        value: enteredValue
      }
    })
    expect(value).to.equal(enteredValue)
    expect(error).to.equal(false)
    expect(showError).to.equal(false)
  })

  it('should store the hyphenated value on change', () => {
    const enteredValue = '110-10-1000'
    let error = false
    let showError = false
    let value = ''
    const setError = errorMessage => {
      error = errorMessage
    }
    const toggleError = show => {
      showError = show
    }
    const inputEntered = inputValue => {
      value = inputValue
    }
    const component = shallow(
      <SsnInput
        error="test error"
        setError={ setError }
        showError={ showError }
        toggleError={ toggleError }
        onInput={ inputEntered }
      />
    )
    component.find('input').simulate('change', {
      target: {
        value: enteredValue
      }
    })
    expect(value).to.equal(enteredValue)
    expect(error).to.equal(false)
    expect(showError).to.equal(false)
  })

  it('should store the value on paste', () => {
    const pasteValue = '110101000'
    let error = false
    let showError = false
    let value = ''
    const setError = errorMessage => {
      error = errorMessage
    }
    const toggleError = show => {
      showError = show
    }
    const inputEntered = inputValue => {
      value = inputValue
    }
    const component = shallow(
      <SsnInput
        error="test error"
        setError={ setError }
        showError={ showError }
        toggleError={ toggleError }
        onInput={ inputEntered }
      />
    )
    component.find('input').simulate('paste', {
      target: {
        value: pasteValue
      },
      clipboardData: {
        getData: text => {
          return pasteValue
        }
      }
    })
    component.find('input').simulate('change',
      {
        target: {
          value: pasteValue
        }
      }
    )
    expect(value).to.equal(pasteValue)
    expect(error).to.equal(false)
    expect(showError).to.equal(false)
  })

  it('should store the hyphenated value on paste', () => {
    const pasteValue = '110-10-1000'
    let error = false
    let showError = false
    let value = ''
    const setError = errorMessage => {
      error = errorMessage
    }
    const toggleError = show => {
      showError = show
    }
    const inputEntered = inputValue => {
      value = inputValue
    }
    const component = shallow(
      <SsnInput
        error="test error"
        setError={ setError }
        showError={ showError }
        toggleError={ toggleError }
        onInput={ inputEntered }
      />
    )
    component.find('input').simulate('paste', {
      target: {
        value: pasteValue
      },
      clipboardData: {
        getData: text => {
          return pasteValue
        }
      }
    })
    component.find('input').simulate('change',
      {
        target: {
          value: pasteValue
        }
      }
    )
    expect(value).to.equal(pasteValue)
    expect(error).to.equal(false)
    expect(showError).to.equal(false)
  })

  it('should not set errors when the SSN is blank', () => {
    let error = false
    let showError = false
    const setError = errorMessage => {
      error = errorMessage
    }
    const toggleError = show => {
      showError = show
    }
    const component = shallow(
      <SsnInput error="test error" setError={ setError } showError={ showError } toggleError={ toggleError } />
    )
    component.find('input').simulate('blur', {
      target: {
        value: ''
      }
    })
    expect(error).to.equal(false)
  })

  it('should not set errors for valid digit-only social security numbers', () => {
    let error = false
    let showError = false
    const setError = errorMessage => {
      error = errorMessage
    }
    const toggleError = show => {
      showError = show
    }
    const component = shallow(
      <SsnInput error="test error" setError={ setError } showError={ showError } toggleError={ toggleError } />
    )
    component.find('input').simulate('blur', {
      target: {
        value: '110101000'
      }
    })
    expect(error).to.equal(false)
  })

  it('should not set errors for valid hyphenated social security numbers', () => {
    const validValues = [ '001-01-0001', '665-01-0001', '667-01-0001', '899-01-0001' ]
    for (var i = 0; i < validValues.length; i++) {
      let error = false
      let showError = false
      const setError = errorMessage => {
        error = errorMessage
      }
      const toggleError = show => {
        showError = show
      }
      const component = shallow(
        <SsnInput error="test error" setError={ setError } showError={ showError } toggleError={ toggleError } />
      )
      component.find('input').simulate('blur', {
        target: {
          value: validValues[i]
        }
      })
      expect(error, 'test value: ' + validValues[i]).to.equal(false)
    }
  })

  it('should set errors for invalid social security numbers', () => {
    const invalidValues = [ 'abcdefg', '000-01-0001', '001-00-0001', '001-01-0000', '666-01-0001', '900-01-0001' ]
    for (var i = 0; i < invalidValues.length; i++) {
      let error = false
      let showError = false
      const setError = errorMessage => {
        error = errorMessage
      }
      const toggleError = show => {
        showError = show
      }

      const component = shallow(
        <SsnInput error="test error" showError={ showError } setError={ setError } toggleError={ toggleError } />
      )
      component.find('input').simulate('blur', {
        target: {
          value: invalidValues[i]
        }
      })
      expect(error, 'test value: ' + invalidValues[i]).to.not.equal(false)
    }
  })

  it('should set errors for SSN with all digits the same', () => {
    let error = false
    let showError = false
    const setError = errorMessage => {
      error = errorMessage
    }
    const toggleError = show => {
      showError = show
    }
    const component = shallow(
      <SsnInput showError={ showError } error="test error" setError={ setError } toggleError={ toggleError } />
    )
    component.find('input').simulate('blur', {
      target: {
        value: '555-55-5555'
      }
    })
    expect(error).to.not.equal(false)
  })

  it('should not display errors if error is false', () => {
    const component = shallow(<SsnInput showError={ false } error={ false } />)
    expect(component.find('span.error')).to.have.length(0)
  })

  it('should not display errors if there is no error message', () => {
    const component = shallow(<SsnInput error={ false } />)
    expect(component.find('span.error')).to.have.length(0)
  })

  it('should display errors if showError is true and there is an error message', () => {
    const component = shallow(<SsnInput error="test error" showError={ true } />)
    expect(component.find('span.error')).to.have.length(1)
  })
})
