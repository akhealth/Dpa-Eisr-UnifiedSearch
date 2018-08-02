/* global describe, it */

import '../../react-mocha-setup'

import React from 'react'
import { shallow } from 'enzyme'
import { expect } from 'chai'

import { EisClientIdInput } from '-/components/inputs/EisClientIdInput'

describe('EisClientIdInput Component', () => {
  it('should store the value on change', () => {
    const enteredValue = '0600000000'
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
      <EisClientIdInput
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
    const pasteValue = '0600000000'
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
      <EisClientIdInput
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

  it('should not set errors for valid Eis Client IDs', () => {
    let error = false
    let showError = false
    const setError = errorMessage => {
      error = errorMessage
    }
    const toggleError = show => {
      showError = show
    }
    const component = shallow(
      <EisClientIdInput error="test error" setError={ setError } showError={ showError } toggleError={ toggleError } />
    )
    component.find('input').simulate('blur', {
      target: {
        value: '0600000000'
      }
    })
    expect(error).to.equal(false)
    expect(showError).to.equal(false)
  })

  it('should set errors for invalid Eis Client IDs', () => {
    let error = false
    let showError = false
    const setError = errorMessage => {
      error = errorMessage
    }
    const toggleError = show => {
      showError = show
    }
    const component = shallow(
      <EisClientIdInput error="test error" setError={ setError } showError={ showError } toggleError={ toggleError } />
    )
    component.find('input').simulate('blur', {
      target: {
        value: 'abcdefg'
      }
    })
    expect(error).to.not.equal(false)
    expect(showError).to.equal(true)
  })

  it('should not display errors if error is false', () => {
    const component = shallow(<EisClientIdInput showError={ false } error={ false } />)
    expect(component.find('span.error')).to.have.length(0)
  })

  it('should not display errors if there is no error message', () => {
    const component = shallow(<EisClientIdInput error={ false } />)
    expect(component.find('span.error')).to.have.length(0)
  })

  it('should display errors if showError is true and there is an error message', () => {
    const component = shallow(<EisClientIdInput error="test error" showError={ true } />)
    expect(component.find('span.error')).to.have.length(1)
  })
})
