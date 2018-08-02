/* global describe, it, beforeEach */

import '../react-mocha-setup'

import { expect } from 'chai'
import nock from 'nock'
import Baobab from 'baobab'

import state from '-/state'
import { getUserInfo } from '-/actions/user'

describe('User Actions', () => {
  const initialState = new Baobab(state.deepClone())
  const username = state.select('user', 'username')

  beforeEach(() => {
    state.deepMerge(initialState.deepClone())
  })

  it('should handle a successful response from the api', async () => {
    const fixture = {
      user: 'Developer'
    }

    nock(global.API_URL)
      .get('/user')
      .reply(200, { success: true, data: fixture })

    await getUserInfo()
    expect(username.get()).to.equal(fixture.user)
  })

  it('should handle a failed response from the api', async () => {
    state.set([ 'config', 'suppressLogging' ], true)
    nock(global.API_URL)
      .get('/user')
      .reply(200, { success: false, data: {} })

    await getUserInfo()
    // TODO: Test error handling when it is added to the getUserInfo method
    expect(username.get()).to.equal(null)
  })

  it('should handle an error response from the api', async () => {
    state.set([ 'config', 'suppressLogging' ], true)
    nock(global.API_URL)
      .get('/user')
      .reply(500, 'Internal server error')

    await getUserInfo()
    // TODO: Test error handling when it is added to the getUserInfo method
    expect(username.get()).to.equal(null)
  })
})
