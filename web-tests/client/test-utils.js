import nock from 'nock'

export const addUserNock = (username = 'Developer') => {
  return nock(global.API_URL)
    .get('/user')
    .reply(200, { success: true, data: { user: username } })
}
