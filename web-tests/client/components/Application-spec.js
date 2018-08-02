/* global describe, it */

import '../react-mocha-setup'

import React from 'react'
import { mount } from 'enzyme'
import { expect } from 'chai'
import { BrowserRouter as Router } from 'react-router-dom'
import { addUserNock } from '../test-utils'

import Application from '-/components/Application'

describe('Unified Search Application', () => {
  it('should run a basic sanity test to ensure tests are working', () => {
    return expect(1).to.equal(1)
  })

  it('should render the Application component', () => {
    addUserNock()
    const component = mount(<Router>{<Application />}</Router>)
    expect(component.find('#app-component')).to.have.length(1)
  })
})
