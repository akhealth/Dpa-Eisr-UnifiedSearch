/* global describe, it, beforeEach */

import '../react-mocha-setup'

import { expect } from 'chai'
import nock from 'nock'
import Baobab from 'baobab'
import { createMemoryHistory } from 'history'

import state from '-/state'
import {
  performSearch,
  resetSearchCount,
  showValidationError,
  setValidationError,
  ariesInputEntered,
  eisInputEntered,
  ssnInputEntered,
  lastNameInputEntered,
  firstNameInputEntered
} from '-/actions/search'
import { setHistory } from '-/actions/session'

describe('Search Actions', () => {
  const initialState = new Baobab(state.deepClone())
  const searchPage = state.select('searchPage')

  beforeEach(() => {
    state.deepMerge(initialState.deepClone())
  })

  it('should show an error for an invalid ssn', async () => {
    const firstName = ''
    const lastName = ''
    const ssn = 'thisShoul'

    nock(global.API_URL)
      .get('/search/getmci')
      .query({
        firstName,
        lastName,
        registration: ssn
      })
      .reply(200, { success: false, data: null })

    await performSearch(firstName, lastName, ssn)
    expect(searchPage.get('requestError')).to.equal(true)
  })

  it('should show an error for an invalid first and last name', async () => {
    const firstName =
      'Thisisover40CharactersThisisover40CharactersThisisover40CharactersThisisover40CharactersThisisover40Characters'
    const lastName =
      'Thisisover40CharactersThisisover40CharactersThisisover40CharactersThisisover40CharactersThisisover40Characters'
    const ssn = ''

    nock(global.API_URL)
      .get('/search/getmci')
      .query({
        firstName,
        lastName,
        registration: ssn
      })
      .reply(200, { success: false, data: null })

    await performSearch(firstName, lastName, ssn)
    expect(searchPage.get('requestError')).to.equal(true)
  })

  it('should show an error when the API responds with internal server error', async () => {
    state.set([ 'config', 'suppressLogging' ], true)
    const firstName = ''
    const lastName = ''
    const ssn = ''

    nock(global.API_URL)
      .get('/search/getmci')
      .query({
        firstName,
        lastName,
        registration: ssn
      })
      .replyWithError('Something bad happened!')

    await performSearch(firstName, lastName, ssn)
    expect(searchPage.get('requestError')).to.equal(true)
  })

  it('should set timed out flag when the ESB request times out', async () => {
    state.set([ 'config', 'suppressLogging' ], true)
    const firstName = ''
    const lastName = ''
    const ssn = ''

    nock(global.API_URL)
      .get('/search/getmci')
      .query({
        firstName,
        lastName,
        registration: ssn
      })
      .reply(400, { success: false, data: { error: 'The operation has timed out.' } })

    await performSearch(firstName, lastName, ssn)
    expect(searchPage.get('timedOut')).to.equal(true)
  })

  it('should set timed out flag when the API request times out', async () => {
    state.set([ 'config', 'suppressLogging' ], true)
    state.set([ 'config', 'requestTimeout' ], 1)
    const firstName = ''
    const lastName = ''
    const ssn = ''

    nock(global.API_URL)
      .get('/search/getmci')
      .query({
        firstName,
        lastName,
        registration: ssn
      })
      .delay(2)
      .reply(400, { success: false, data: null })

    await performSearch(firstName, lastName, ssn)
    expect(searchPage.get('timedOut')).to.equal(true)
  })

  it('should perform an empty search', async () => {
    const firstName = ''
    const lastName = ''
    const ssn = ''

    nock(global.API_URL)
      .get('/search/getmci')
      .query({
        firstName,
        lastName,
        registration: ssn
      })
      .reply(200, { success: true, data: null })

    await performSearch(firstName, lastName, ssn)
    expect(searchPage.get('searchResults')).to.equal(null)
  })

  it('should search by SSN', async () => {
    const fixture = [
      {
        VirtualId: '6958302',
        MatchPercentage: null,
        Title: null,
        FirstName: 'Denise',
        MiddleName: null,
        LastName: 'Snow',
        Suffix: null,
        DateOfBirth: '1979-01-31T09:00:00.000Z',
        Gender: 'Female',
        registrations: {
          registration: [
            { registrationName: 'ARIES_ID', registrationValue: '2000006624' },
            { registrationName: 'SSN', registrationValue: '065432198' }
          ]
        },
        Names: {
          Name: [
            {
              NameType: 'Registered',
              Title: null,
              FirstName: 'Denise',
              MiddleName: null,
              LastName: 'Snow',
              Suffix: null
            }
          ]
        }
      }
    ]
    const firstName = ''
    const lastName = ''
    const ssn = '065432198'

    nock(global.API_URL)
      .get('/search/getmci')
      .query({
        firstName,
        lastName,
        registration: ssn
      })
      .reply(200, { success: true, data: fixture })

    await performSearch(firstName, lastName, ssn)
    expect(searchPage.get('searchResults')[0].VirtualId).to.equal(fixture[0].VirtualId)
  })

  it('should search by first and last name', async () => {
    const fixture = [
      {
        VirtualId: '6959438',
        MatchPercentage: null,
        Title: null,
        FirstName: 'Orora',
        MiddleName: null,
        LastName: 'Snow',
        Suffix: null,
        DateOfBirth: '2000-01-28T00:00:00',
        Gender: 'Female',
        registrations: {
          registration: [
            { registrationName: 'ARIES_ID', registrationValue: '2400000387' },
            { registrationName: 'SSN', registrationValue: '269858585' }
          ]
        },
        Names: {
          Name: [
            {
              NameType: 'Registered',
              Title: null,
              FirstName: 'Orora',
              MiddleName: null,
              LastName: 'Snow',
              Suffix: null
            }
          ]
        }
      }
    ]
    const firstName = 'Orora'
    const lastName = 'Snow'
    const ssn = ''

    nock(global.API_URL)
      .get('/search/getmci')
      .query({
        firstName,
        lastName,
        registration: ssn
      })
      .reply(200, { success: true, data: fixture })

    await performSearch(firstName, lastName, ssn)
    expect(searchPage.get('searchResults')[0].VirtualId).to.equal(fixture[0].VirtualId)
  })

  it('should jump to a person when searching by SSN', async () => {
    const fixture = [
      {
        VirtualId: '6958302',
        MatchPercentage: null,
        Title: null,
        FirstName: 'Denise',
        MiddleName: null,
        LastName: 'Snow',
        Suffix: null,
        DateOfBirth: '1979-01-31T09:00:00.000Z',
        Gender: 'Female',
        registrations: {
          registration: [
            { registrationName: 'ARIES_ID', registrationValue: '2000006624' },
            { registrationName: 'SSN', registrationValue: '065432198' }
          ]
        },
        Names: {
          Name: [
            {
              NameType: 'Registered',
              Title: null,
              FirstName: 'Denise',
              MiddleName: null,
              LastName: 'Snow',
              Suffix: null
            }
          ]
        }
      }
    ]
    const firstName = ''
    const lastName = ''
    const ssn = '065432198'
    const history = createMemoryHistory()
    setHistory(history)
    searchPage.set('jumpToPerson', null)

    nock(global.API_URL)
      .get('/search/getmci')
      .query({
        firstName,
        lastName,
        registration: ssn
      })
      .reply(200, { success: true, data: fixture })

    await performSearch(firstName, lastName, ssn)
    expect(searchPage.get('jumpToPerson')).to.equal(fixture[0].registrations.registration[1].registrationValue)
  })

  it('should reset first search', () => {
    searchPage.set('isFirstSearch', false)
    resetSearchCount()
    expect(searchPage.get('isFirstSearch')).to.equal(true)
  })

  it('should set a validation error', () => {
    const key = 'ariesClientId'
    const error = 'Test Error'
    setValidationError(key, error)
    expect(searchPage.get([ 'validation', 'errors', key ])).to.equal(error)
  })

  it('should show a validation error', () => {
    const key = 'ariesClientId'
    showValidationError(key, true)
    expect(searchPage.get([ 'validation', 'showErrors', key ])).to.equal(true)
  })

  describe('ARIES Client ID input actions', () => {
    it('should disable other ID inputs when an ARIES Client ID is entered', () => {
      const value = '2000000000'
      ariesInputEntered('2000000000')
      expect(searchPage.get([ 'formValues', 'ariesClientId' ])).to.equal(value)
      expect(searchPage.get([ 'enabledFields', 'eisClientIdInputEnabled' ])).to.equal('disabled')
      expect(searchPage.get([ 'enabledFields', 'ssnInputEnabled' ])).to.equal('disabled')
    })

    it('should not disable other ID inputs when the ARIES Client ID is blank', () => {
      ariesInputEntered('')
      expect(searchPage.get([ 'enabledFields', 'eisClientIdInputEnabled' ])).to.equal('')
      expect(searchPage.get([ 'enabledFields', 'ssnInputEnabled' ])).to.equal('')
    })

    it('should show an EIS client ID error', () => {
      const eisKey = 'eisClientId'
      setValidationError(eisKey, 'Error')
      ariesInputEntered('')
      expect(searchPage.get([ 'validation', 'showErrors', eisKey ])).to.equal(true)
    })

    it('should show an invalid SSN error', () => {
      const ssnKey = 'ssn'
      setValidationError(ssnKey, 'Error')
      ariesInputEntered('')
      expect(searchPage.get([ 'validation', 'showErrors', ssnKey ])).to.equal(true)
    })
  })

  describe('EIS Client ID input actions', () => {
    it('should disable other ID inputs when an EIS Client ID is entered', () => {
      const value = '0600000000'
      eisInputEntered('0600000000')
      expect(searchPage.get([ 'formValues', 'eisClientId' ])).to.equal(value)
      expect(searchPage.get([ 'enabledFields', 'ariesClientIdInputEnabled' ])).to.equal('disabled')
      expect(searchPage.get([ 'enabledFields', 'ssnInputEnabled' ])).to.equal('disabled')
    })

    it('should not disable other ID inputs when the EIS Client ID is blank', () => {
      eisInputEntered('')
      expect(searchPage.get([ 'enabledFields', 'ariesClientIdInputEnabled' ])).to.equal('')
      expect(searchPage.get([ 'enabledFields', 'ssnInputEnabled' ])).to.equal('')
    })

    it('should show an ARIES client ID error', () => {
      const ariesKey = 'ariesClientId'
      setValidationError(ariesKey, 'Error')
      eisInputEntered('')
      expect(searchPage.get([ 'validation', 'showErrors', ariesKey ])).to.equal(true)
    })

    it('should show an invalid SSN error', () => {
      const ssnKey = 'ssn'
      setValidationError(ssnKey, 'Error')
      eisInputEntered('')
      expect(searchPage.get([ 'validation', 'showErrors', ssnKey ])).to.equal(true)
    })
  })

  describe('SSN input actions', () => {
    it('should disable other ID inputs when a SSN is entered', () => {
      const value = '110101000'
      ssnInputEntered(value)
      expect(searchPage.get([ 'formValues', 'ssn' ])).to.equal(value)
      expect(searchPage.get([ 'enabledFields', 'ariesClientIdInputEnabled' ])).to.equal('disabled')
      expect(searchPage.get([ 'enabledFields', 'eisClientIdInputEnabled' ])).to.equal('disabled')
    })

    it('should not disable other ID inputs when the SSN is blank', () => {
      ssnInputEntered('')
      expect(searchPage.get([ 'enabledFields', 'ariesClientIdInputEnabled' ])).to.equal('')
      expect(searchPage.get([ 'enabledFields', 'eisClientIdInputEnabled' ])).to.equal('')
    })

    it('should show an ARIES client ID error', () => {
      const ariesKey = 'ariesClientId'
      setValidationError(ariesKey, 'Error')
      ssnInputEntered('')
      expect(searchPage.get([ 'validation', 'showErrors', ariesKey ])).to.equal(true)
    })

    it('should show an EIS client ID error', () => {
      const eisKey = 'eisClientId'
      setValidationError(eisKey, 'Error')
      ssnInputEntered('')
      expect(searchPage.get([ 'validation', 'showErrors', eisKey ])).to.equal(true)
    })
  })

  it('should save the last name value', () => {
    const value = 'testLastName'
    lastNameInputEntered(value)
    expect(searchPage.get([ 'formValues', 'lastName' ])).to.equal(value)
  })

  it('should save the first name value', () => {
    const value = 'testFirstName'
    firstNameInputEntered(value)
    expect(searchPage.get([ 'formValues', 'firstName' ])).to.equal(value)
  })
})
