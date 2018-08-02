/* global describe, it, beforeEach */

import '../react-mocha-setup'

import { expect } from 'chai'
import nock from 'nock'
import Baobab from 'baobab'

import state from '-/state'
import { getPersonDetails, clearPersonDetails } from '-/actions/person'

describe('Person Actions', () => {
  const initialState = new Baobab(state.deepClone())
  const personPage = state.select('personDetailsPage')

  beforeEach(() => {
    state.deepMerge(initialState.deepClone())
  })

  it('should handle a successful response from the api', async () => {
    const fixture = {
      personalInfo: {
        name: 'Alan T. Johnson',
        ssn: '987-65-4321',
        dob: '05/04/1972'
      },
      systemInfo: {
        eisClientIds: [ '0675634429', '0677348933' ],
        ariesClientIds: [ '2442906756' ]
      },
      cases: []
    }

    nock(global.API_URL)
      .get('/person/2442906756')
      .reply(200, { success: true, data: fixture })

    await getPersonDetails('2442906756')
    expect(personPage.get('personDetailsData')).to.deep.equal(fixture)
  })

  it('should clear person details data', () => {
    clearPersonDetails()
    expect(personPage.get('personDetailsData')).to.equal(null)
  })

  it('should handle a failed response from the api', async () => {
    nock(global.API_URL)
      .get('/person/2442906756')
      .reply(200, { success: false, data: {} })

    await getPersonDetails('2442906756')
    expect(personPage.get('requestError')).to.equal(true)
  })

  it('should handle an error response from the api', async () => {
    state.set([ 'config', 'suppressLogging' ], true)
    nock(global.API_URL)
      .get('/person/2442906756')
      .reply(500, 'Internal server error')

    await getPersonDetails('2442906756')
    expect(personPage.get('requestError')).to.equal(true)
  })

  it('should set timed out flag when the ESB request times out', async () => {
    state.set([ 'config', 'suppressLogging' ], true)
    nock(global.API_URL)
      .get('/person/2442906756')
      .reply(400, { success: false, data: { error: 'The operation has timed out.' } })

    await getPersonDetails('2442906756')
    expect(personPage.get('timedOut')).to.equal(true)
  })

  it('should set timed out flag when the API request times out', async () => {
    state.set([ 'config', 'suppressLogging' ], true)
    state.set([ 'config', 'requestTimeout' ], 1)

    nock(global.API_URL)
      .get('/person/2442906756')
      .delay(2)
      .reply(400, { success: false, data: null })

    await getPersonDetails('2442906756')
    expect(personPage.get('timedOut')).to.equal(true)
  })
})
