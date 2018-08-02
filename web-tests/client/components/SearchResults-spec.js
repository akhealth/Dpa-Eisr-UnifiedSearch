/* global describe, it */

import '../react-mocha-setup'

import React from 'react'
import { shallow, mount } from 'enzyme'
import { expect } from 'chai'
import { createMemoryHistory } from 'history'
import { Router } from 'react-router-dom'

import { testLinkedComponent, report } from '../axe-helpers'
import { SearchResults } from '-/components/SearchResults/SearchResults'

const mountWithRouter = component => {
  const history = createMemoryHistory()
  return mount(<Router history={ history }>{component}</Router>)
}

describe('SearchResults Component', () => {
  const fixture = [
    {
      lastName: 'lastName',
      firstName: 'firstName',
      middleName: 'middleName',
      registrations: {
        registration: [
          {
            registrationName: 'EIS_ID',
            registrationValue: '0'
          },
          {
            registrationName: 'ARIES_ID',
            registrationValue: '0'
          },
          {
            registrationName: 'SSN',
            registrationValue: '111111111'
          }
        ]
      },
      dateOfBirth: '01/01/1900'
    },
    {
      lastName: 'lastName',
      firstName: 'firstName',
      middleName: '',
      registrations: {
        registration: [
          {
            registrationName: 'SSN',
            registrationValue: '0'
          }
        ]
      },
      dateOfBirth: '01/01/1900'
    }
  ]

  it('should render', () => {
    const component = shallow(<SearchResults />)
    expect(component.find('.search-results')).to.have.length(1)
  })

  it('should show a message to enter search criteria', () => {
    const component = shallow(<SearchResults isFirstSearch={ true } />)
    expect(component.find('.no-results')).to.have.length(2)
  })

  it('should indicate when a search is running', () => {
    const component = shallow(<SearchResults searchRunning={ true } />)
    expect(component.find('.search-results')).to.have.length(1)
  })

  it('should show a message that the request timed out', () => {
    const component = shallow(<SearchResults timedOut={ true } />)
    expect(component.find('#timed-out-message')).to.have.length(1)
  })

  it('should show a message that the results failed to load', () => {
    const component = shallow(<SearchResults requestError={ true } />)
    expect(component.find('#request-error-message')).to.have.length(1)
  })

  it('should render a results table when there are results', () => {
    const component = shallow(<SearchResults searchResults={ fixture } />)
    expect(component.find('table')).to.have.length(1)
  })

  it('should NOT render a results table when there are no results', () => {
    const component = shallow(<SearchResults searchResults={ [] } />)
    expect(component.find('table')).to.have.length(0)
  })

  it('should render a results table when there is no SSN', () => {
    const noSsnFixture = JSON.parse(JSON.stringify(fixture))
    noSsnFixture[0].registrations.registration = noSsnFixture[0].registrations.registration.filter(
      r => r.registrationName !== 'SSN'
    )

    const component = mountWithRouter(<SearchResults searchResults={ noSsnFixture } />)
    expect(component.find('table')).to.have.length(1)
  })

  it('should render a results table when there is no ARIES_ID', () => {
    const noAriesIdFixture = JSON.parse(JSON.stringify(fixture))
    noAriesIdFixture[0].registrations.registration = noAriesIdFixture[0].registrations.registration.filter(
      r => r.registrationName !== 'ARIES_ID'
    )

    const component = mountWithRouter(<SearchResults searchResults={ noAriesIdFixture } />)
    expect(component.find('table')).to.have.length(1)
  })

  it('should render a results table when there is no EIS_ID', () => {
    const noEisIdFixture = JSON.parse(JSON.stringify(fixture))
    noEisIdFixture[0].registrations.registration = noEisIdFixture[0].registrations.registration.filter(
      r => r.registrationName !== 'EIS_ID'
    )

    const component = mountWithRouter(<SearchResults searchResults={ noEisIdFixture } />)
    expect(component.find('table')).to.have.length(1)
  })

  it('should render a results table when there is no ARIES_ID or SSN', () => {
    const noAriesSSNFixture = JSON.parse(JSON.stringify(fixture))
    noAriesSSNFixture[0].registrations.registration = noAriesSSNFixture[0].registrations.registration.filter(
      r => r.registrationName !== 'ARIES_ID' && r.registrationName !== 'SSN'
    )

    const component = mountWithRouter(<SearchResults searchResults={ noAriesSSNFixture } />)
    expect(component.find('table')).to.have.length(1)
  })

  it('should render a results table when there are no registrations', () => {
    const noRegistrationFixture = JSON.parse(JSON.stringify([ fixture[0] ]))
    noRegistrationFixture[0].registrations.registration = []

    const component = mountWithRouter(<SearchResults searchResults={ noRegistrationFixture } />)
    expect(component.find('table')).to.have.length(1)
  })

  it('should show a warning when there are too many results', () => {
    let recordArray = []
    const addRecord = JSON.parse(JSON.stringify(fixture[0]))
    for (var i = 0; i < 51; i++) {
      recordArray.push(addRecord)
    }
    const component = shallow(<SearchResults searchResults={ recordArray } tooManyResults={ true } />)
    expect(component.find('#too-many-results')).to.have.length(1)
  })

  it('should not have accessibility errors', () => {
    return testLinkedComponent(<SearchResults />).then(({ violations }) => {
      expect(violations.length).to.equal(0, report(violations))
    })
  })

  it('should not have accessibility errors when there are results', () => {
    return testLinkedComponent(<SearchResults searchResults={ [ fixture[0] ] } />).then(({ violations }) => {
      expect(violations.length).to.equal(0, report(violations))
    })
  })
})
