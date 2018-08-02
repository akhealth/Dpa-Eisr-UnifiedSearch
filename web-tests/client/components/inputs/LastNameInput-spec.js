/* global describe, it */

import '../../react-mocha-setup'

import React from 'react'
import { shallow } from 'enzyme'
import { expect } from 'chai'

import { LastNameInput } from '-/components/inputs/LastNameInput'

describe('LastNameInput Component', () => {
  it('should not allow inputs > maxLength', () => {
    let error = false
    let showError = false
    const setError = errorMessage => {
      error = errorMessage
    }
    const toggleError = show => {
      showError = show
    }
    const maxLength = 40
    const component = shallow(
      <LastNameInput
        maxLength={ maxLength }
        error="test"
        setError={ setError }
        toggleError={ toggleError }
        onInput={ () => '' }
      />
    )
    component.find('input').simulate('change', {
      target: {
        value: new Array(maxLength + 1).join('a')
      }
    })
    expect(error).to.not.equal(false)
  })

  it('should not allow pasted inputs > maxLength', () => {
    const maxLength = 40
    const pasteValue = new Array(maxLength + 1).join('a')
    let error = false
    let showError = false
    const setError = errorMessage => {
      error = errorMessage
    }
    const toggleError = show => {
      showError = show
    }
    const component = shallow(
      <LastNameInput
        maxLength={ maxLength }
        error="test"
        setError={ setError }
        toggleError={ toggleError }
        onInput={ () => '' }
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
    component.find('input').simulate('change', {
      target: {
        value: pasteValue
      }
    })
    expect(error).to.not.equal(false)
  })

  it('should allow inputs <= maxLength', () => {
    let error = false
    let showError = false
    const setError = errorMessage => {
      error = errorMessage
    }
    const toggleError = show => {
      showError = show
    }
    const maxLength = 40
    const component = shallow(
      <LastNameInput
        maxLength={ maxLength }
        error="test"
        setError={ setError }
        toggleError={ toggleError }
        onInput={ () => '' }
      />
    )
    component.find('input').simulate('change', {
      target: {
        value: new Array(maxLength).join('a')
      }
    })
    expect(error).to.equal(false)
  })

  it('should display an error message', () => {
    const component = shallow(<LastNameInput maxLength="40" error="test" showError={ true } />)
    expect(component.find('span.error')).to.have.length(1)
  })

  it('should not display an error message when there is no error', () => {
    const component = shallow(<LastNameInput maxLength="40" error={ false } />)
    expect(component.find('span.error')).to.have.length(0)
  })
})
