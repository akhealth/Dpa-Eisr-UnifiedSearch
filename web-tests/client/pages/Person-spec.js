/* global describe, it */

import '../react-mocha-setup'

import React from 'react'
import { shallow, mount } from 'enzyme'
import { expect } from 'chai'

import { testComponent, report } from '../axe-helpers'
import { Person, PersonHeader, CaseSummary, CasePrograms, ApplicationSummary } from '-/pages/Person/Person'

describe('Person Page', () => {
  const fixture = {
    personalInfo: {
      firstName: 'Alan',
      name: 'Alan T. Johnson',
      ssns: [ '987654321' ],
      dob: '05/04/1972'
    },
    systemInfo: {
      eisClientIds: [ '0675634429', '0677348933' ],
      ariesClientIds: [ '2442906756' ]
    },
    applications: [
      {
        applicationNumber: 'T32256754',
        status: 'Application Received',
        receivedDate: '03/03/2018'
      },
      {
        applicationNumber: 'T32256755',
        status: 'Application Approved',
        receivedDate: '03/04/2018'
      },
      {
        applicationNumber: 'T32256756',
        status: 'NA',
        receivedDate: '03/05/2018'
      }
    ],
    cases: [
      {
        location: 'ARIES',
        caseNumber: '31234567',
        clientId: '2442906756',
        primaryIndividual: {
          name: 'Alan T. Johnson',
          clientId: '2442906756'
        },
        programs: [
          {
            programName: 'ME',
            programStatus: 'Open',
            medicaidSubType: 'MAGI - Pregnant Woman',
            eligibilityCode: '11',
            programSubtype: 'PW',
            lastIssued: '02/01/2018',
            benefit: '',
            type: '',
            issuances: []
          }
        ]
      },
      {
        location: 'ARIES',
        caseNumber: '31234567',
        clientId: '2442906756',
        primaryIndividual: {
          name: 'Sarah B Johnson',
          clientId: '2657738299'
        },
        programs: []
      },
      {
        location: 'EIS',
        caseNumber: '12345678',
        clientId: '2442906756',
        primaryIndividual: {
          name: 'Sarah B Johnson',
          clientId: '2657738299'
        },
        programs: [
          {
            programName: 'AP',
            programStatus: 'Open',
            programSubtype: 'OA',
            lastIssued: '02/01/2018',
            benefit: '$100',
            type: 'warrant',
            issuances: [
              {
                issuanceType: 'warrant'
              }
            ]
          },
          {
            programName: 'AF',
            programStatus: 'Closed',
            lastIssued: '02/01/2018',
            benefit: '$100',
            issuances: []
          },
          {
            programName: 'ME',
            programStatus: 'NA',
            medicaidSubType: 'AF',
            eligibilityCode: '11',
            programSubtype: 'PW',
            lastIssued: '02/01/2018',
            benefit: '',
            type: '',
            issuances: []
          }
        ]
      },
      {
        location: 'EIS',
        caseNumber: '87654321',
        clientId: '2442907576',
        primaryIndividual: {
          name: 'Sarah B Johnson',
          clientId: '2657738299'
        },
        programs: [
          {
            programName: 'GA',
            programStatus: 'Open',
            programSubtype: 'OA',
            lastIssued: '02/01/2018',
            benefit: '$100',
            type: 'warrant',
            issuances: [
              {
                issuanceAmount: '$100'
              }
            ]
          },
          {
            programName: 'GA',
            programStatus: 'Open',
            programSubtype: 'OA',
            lastIssued: '02/01/2018',
            benefit: '$100',
            type: 'warrant',
            issuances: [
              {
                issuanceDateString: '02/01/2018'
              }
            ]
          }
        ]
      }
    ]
  }
  const match = {
    params: {
      personId: '2442906756'
    }
  }

  it('should render', () => {
    const component = shallow(<Person personDetailsData={ null } match={ match } />)
    expect(component).to.have.length(1)
  })

  it('should show a message when loading data', () => {
    const component = shallow(<Person personDetailsData={ null } match={ match } />)
    expect(component.find('#loading-message')).to.have.length(1)
  })

  it('should not show a loading message when it has data', () => {
    const component = shallow(<Person personDetailsData={ fixture } match={ match } />)
    expect(component.find('#loading-message')).to.have.length(0)
  })

  it('should show a message that the request timed out', () => {
    const component = shallow(<Person personDetailsData={ null } timedOut={ true } match={ match } />)
    expect(component.find('#timed-out')).to.have.length(1)
  })

  it('should show a message that the page failed to load', () => {
    const component = mount(
      <Person personDetailsData={ null } timedOut={ false } requestError={ true } match={ match } />
    )
    expect(component.find('.error-message')).to.have.length(1)
  })

  it('should render the person header', () => {
    const component = shallow(<PersonHeader personalInfo={ fixture.personalInfo } systemInfo={ fixture.systemInfo } />)
    expect(component.find('.person-header')).to.have.length(1)
  })

  it('should show a message that there are no applications', () => {
    const component = mount(<ApplicationSummary applications={ [] } />)
    expect(component.find('.no-applicatons')).to.have.length(1)
  })

  it('should render the application summary', () => {
    const component = mount(<ApplicationSummary applications={ fixture.applications } />)
    expect(component.find('.application-summary')).to.have.length.greaterThan(0)
  })

  it('should highlight one application status in blue and one in gray', () => {
    const component = mount(<ApplicationSummary applications={ fixture.applications } />)
    expect(component.find('.blue-value')).to.have.length(1)
    expect(component.find('.gray-value')).to.have.length(1)
  })

  it('should render an ARIES case summary', () => {
    const component = shallow(<CaseSummary caseInfo={ fixture.cases[0] } personalInfo={ fixture.personalInfo } />)
    expect(component.find('.case-summary')).to.have.length(1)
  })

  it('should render an EIS case summary', () => {
    const component = shallow(<CaseSummary caseInfo={ fixture.cases[2] } personalInfo={ fixture.personalInfo } />)
    expect(component.find('.case-summary')).to.have.length(1)
  })

  it('should show a message that there are no programs', () => {
    const component = mount(<CasePrograms programs={ [] } personalInfo={ fixture.personalInfo } />)
    expect(component.find('.case-no-programs')).to.have.length(1)
  })

  it('should render a program', () => {
    const component = mount(
      <CasePrograms programs={ fixture.cases[0].programs } personalInfo={ fixture.personalInfo } />
    )
    expect(component.find('.case-program-container')).to.have.length(1)
  })

  it('should render three programs', () => {
    const component = mount(
      <CasePrograms programs={ fixture.cases[2].programs } personalInfo={ fixture.personalInfo } />
    )
    expect(component.find('.case-program-container')).to.have.length(3)
  })

  it('should render a program with an eligibility code', () => {
    const component = mount(
      <CasePrograms programs={ fixture.cases[0].programs } personalInfo={ fixture.personalInfo } />
    )
    expect(component.find('.program-eligibility')).to.have.length(1)
  })

  it('should render a program without an eligibility code', () => {
    const component = mount(
      <CasePrograms programs={ [ fixture.cases[2].programs[0] ] } personalInfo={ fixture.personalInfo } />
    )
    expect(component.find('.program-eligibility')).to.have.length(0)
  })

  it('should render a program with a subtype', () => {
    const component = mount(
      <CasePrograms programs={ fixture.cases[0].programs } personalInfo={ fixture.personalInfo } />
    )
    expect(component.find('.program-subType')).to.have.length(1)
  })

  it('should render a program without a subtype', () => {
    const component = mount(
      <CasePrograms programs={ [ fixture.cases[2].programs[0] ] } personalInfo={ fixture.personalInfo } />
    )
    expect(component.find('.program-subType')).to.have.length(0)
  })

  it('should render a program with a type', () => {
    const component = mount(
      <CasePrograms programs={ [ fixture.cases[2].programs[0] ] } personalInfo={ fixture.personalInfo } />
    )
    expect(component.find('.program-type')).to.have.length(1)
  })

  it('should render a program without a type', () => {
    const component = mount(
      <CasePrograms programs={ fixture.cases[0].programs } personalInfo={ fixture.personalInfo } />
    )
    expect(component.find('.program-type')).to.have.length(0)
  })

  it('should render an issuance with an issuance amount', () => {
    const component = mount(
      <CasePrograms programs={ [ fixture.cases[3].programs[0] ] } personalInfo={ fixture.personalInfo } />
    )
    expect(component.find('.issuance-info')).to.have.length(1)
  })

  it('should render an issuance with an issuance date', () => {
    const component = mount(
      <CasePrograms programs={ [ fixture.cases[3].programs[1] ] } personalInfo={ fixture.personalInfo } />
    )
    expect(component.find('.issuance-info')).to.have.length(1)
  })

  it('should render a program with certification label', () => {
    const component = mount(
      <CasePrograms programs={ fixture.cases[0].programs } personalInfo={ fixture.personalInfo } />
    )
    expect(component.find('.certification-date')).to.have.length(1)
  })

  it('should highlight one program status in blue and one in gray', () => {
    const component = mount(
      <CasePrograms programs={ fixture.cases[2].programs } personalInfo={ fixture.personalInfo } />
    )
    expect(component.find('.blue-status')).to.have.length(1)
    expect(component.find('.gray-status')).to.have.length(1)
  })

  it('should not have accessibility errors', () => {
    return testComponent(<Person personDetailsData={ null } match={ match } />).then(({ violations }) => {
      expect(violations.length).to.equal(0, report(violations))
    })
  })
})
