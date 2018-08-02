/* global describe, it, beforeEach */

import '../react-mocha-setup'

import React from 'react'
import { shallow, mount } from 'enzyme'
import { expect } from 'chai'
import Baobab from 'baobab'
import nock from 'nock'

import { testComponent, report } from '../axe-helpers'
import { Search } from '-/pages/Search/Search'
import state from '-/state'
import { addUserNock } from '../test-utils'

describe('Search Page', () => {
  const initialState = new Baobab(state.deepClone())
  const searchPage = state.select('searchPage')

  beforeEach(() => {
    state.deepMerge(initialState.deepClone())
  })

  const stateFixture = {
    searchResults: [],
    searchRunning: false,
    activeSearchQuery: null,
    validation: {
      errors: {
        ssn: false,
        firstName: false,
        lastName: false,
        ariesClientId: false,
        eisClientId: false
      },
      showErrors: {
        ssn: false,
        ariesClientId: false,
        eisClientId: false
      }
    },
    enabledFields: {
      ssnInputEnabled: '',
      eisClientIdInputEnabled: '',
      ariesClientIdInputEnabled: ''
    },
    formValues: {}
  }

  it('should render', () => {
    addUserNock()
    const component = shallow(<Search { ...stateFixture } />)
    expect(component).to.have.length(1)
  })

  it('should render firstName, lastName, SSN, ARIES, and EIS inputs', () => {
    addUserNock()
    const component = mount(<Search { ...stateFixture } />)
    expect(component.find('#search-firstName')).to.have.length(1)
    expect(component.find('#search-lastName')).to.have.length(1)
    expect(component.find('#search-SSN')).to.have.length(1)
    expect(component.find('#search-eisId')).to.have.length(1)
    expect(component.find('#search-ariesId')).to.have.length(1)
  })

  it('should perform a search using SSN', () => {
    const ssn = '656010001'
    addUserNock()
    nock(global.API_URL)
      .get('/search/getmci')
      .query({
        firstName: '',
        lastName: '',
        registration: ssn
      })
      .reply(200, { success: true, data: null })
    const component = mount(<Search { ...stateFixture } />)

    component
      .find('form')
      .first()
      .simulate('submit', {
        preventDefault: () => { },
        target: {
          searchFirstName: { value: '' },
          searchLastName: { value: '' },
          searchSSN: { value: ssn },
          searchAriesId: { value: '' },
          searchEisId: { value: '' }
        },
        enabledFields: () => { }
      })
    expect(searchPage.get('activeSearchQuery').registration).to.equal(ssn)
  })

  it('should perform a search using ARIES client ID', () => {
    const ariesID = '2400154032'
    addUserNock()
    nock(global.API_URL)
      .get('/search/getmci')
      .query({
        firstName: '',
        lastName: '',
        registration: ariesID
      })
      .reply(200, { success: true, data: null })
    const component = mount(<Search { ...stateFixture } />)

    component
      .find('form')
      .first()
      .simulate('submit', {
        preventDefault: () => { },
        target: {
          searchFirstName: { value: '' },
          searchLastName: { value: '' },
          searchSSN: { value: '' },
          searchAriesId: { value: ariesID },
          searchEisId: { value: '' }
        },
        enabledFields: () => { }
      })
    expect(searchPage.get('activeSearchQuery').registration).to.equal(ariesID)
  })

  it('should perform a search using EIS client ID', () => {
    const eisID = '0600026712'
    addUserNock()
    nock(global.API_URL)
      .get('/search/getmci')
      .query({
        firstName: '',
        lastName: '',
        registration: eisID
      })
      .reply(200, { success: true, data: null })
    const component = mount(<Search { ...stateFixture } />)

    component
      .find('form')
      .first()
      .simulate('submit', {
        preventDefault: () => { },
        target: {
          searchFirstName: { value: '' },
          searchLastName: { value: '' },
          searchSSN: { value: '' },
          searchAriesId: { value: '' },
          searchEisId: { value: eisID }
        },
        enabledFields: () => { }
      })
    expect(searchPage.get('activeSearchQuery').registration).to.equal(eisID)
  })

  it('should perform a search using first name and last name', () => {
    const firstName = 'firstName'
    const lastName = 'lastName'
    addUserNock()
    nock(global.API_URL)
      .get('/search/getmci')
      .query({
        firstName: firstName,
        lastName: lastName,
        registration: ''
      })
      .reply(200, { success: true, data: null })
    const component = mount(<Search { ...stateFixture } />)

    component
      .find('form')
      .first()
      .simulate('submit', {
        preventDefault: () => { },
        target: {
          searchFirstName: { value: firstName },
          searchLastName: { value: lastName },
          searchSSN: { value: '' },
          searchAriesId: { value: '' },
          searchEisId: { value: '' }
        },
        enabledFields: () => { }
      })
    expect(searchPage.get('activeSearchQuery').firstName).to.equal(firstName)
    expect(searchPage.get('activeSearchQuery').lastName).to.equal(lastName)
  })

  it('should not perform a search with just the first name', () => {
    addUserNock()
    const component = mount(<Search { ...stateFixture } />)

    component
      .find('form')
      .first()
      .simulate('submit', {
        preventDefault: () => { },
        target: {
          searchFirstName: { value: 'firstName' },
          searchLastName: { value: '' },
          searchSSN: { value: '' },
          searchAriesId: { value: '' },
          searchEisId: { value: '' }
        },
        enabledFields: () => { }
      })
    expect(searchPage.get('searchRunning')).to.equal(false)
  })

  it('should not perform a search with no search criteria', () => {
    addUserNock()
    const component = mount(<Search { ...stateFixture } />)

    component
      .find('form')
      .first()
      .simulate('submit', {
        preventDefault: () => { },
        target: {
          searchFirstName: { value: '' },
          searchLastName: { value: '' },
          searchSSN: { value: '' },
          searchAriesId: { value: '' },
          searchEisId: { value: '' }
        },
        enabledFields: () => { }
      })
    expect(searchPage.get('searchRunning')).to.equal(false)
  })

  it('should not perform a search when there are validation errors', () => {
    addUserNock()
    const errorFixture = JSON.parse(JSON.stringify(stateFixture))
    errorFixture.validation.showErrors.ssn = true
    const component = mount(<Search { ...errorFixture } />)

    component
      .find('form')
      .first()
      .simulate('submit', {
        preventDefault: () => { },
        target: {
          searchFirstName: { value: 'firstName' },
          searchLastName: { value: 'lastName' },
          searchSSN: { value: 'abcd' },
          searchAriesId: { value: '' },
          searchEisId: { value: '' }
        },
        enabledFields: () => { }
      })
    expect(searchPage.get('searchRunning')).to.equal(false)
  })

  it('should cancel the search', () => {
    addUserNock()
    const runningFixture = JSON.parse(JSON.stringify(stateFixture))
    runningFixture.searchRunning = true
    const component = mount(<Search { ...runningFixture } />)

    component.find('#cancel-search').simulate('click')
    expect(searchPage.get('searchRunning')).to.equal(false)
  })

  it('should not have accessibility errors', () => {
    addUserNock()
    return testComponent(<Search { ...stateFixture } />).then(({ violations }) => {
      expect(violations.length).to.equal(0, report(violations))
    })
  })
})
