# Sprint Logs

## `Sprint 0 (2018/01/10 - 2018/01/23)`

Discovery and project setup.

## `Sprint 1 (2018/01/24 - 2018/02/06)`

This sprint included the following:

- Implemented initial front-end framework
- Created a basic search page which had the ability to search by first name, last name, and SSN. Results were output as JSON
- Connnected the web site to the search API
- Introduced initial integration with the ESB

## `Sprint 2 (2018/02/07 - 2018/02/20)`

The main focus of this sprint was on the UI, which was significantly overhauled from the prototype it was in sprint 1 into a usable UI.

Notable changes include:

- Search page UI was designed and implemented
- Search results were shown in a table
- Site load times were optimized
- Validations were added to search page inputs
- Person details UI was designed and implemented with mocked data
- Gitbook implementation, which provides a clean web interface to view the documentation while running the search application

## `Sprint 3 (2018/02/21 - 2018/03/06)`

This sprint was mostly focused on the person details page. Notable changes include:

- Person details UI was slightly redesigned
- Documentation was significantly updated
- Tests were written/updated
- API was refactored

## `Sprint 4 (2018/03/07 - 2018/03/20)`

This sprint also featured large updates to the Person Details page, including:

- Display of system source for a given record (ARIES vs EIS)
- List of cases associated with a certain client
- Various information on each of these cases, such as: PI name and ID, case status, program type, program subcode, eligibility code, and medicaid subtype
- Error message when user enters an invalid SSN
- Some changes in UI labels
- Further API refactoring

## `Sprint 5 (2018/03/21 - 2018/04/03)`

This sprint centered around two things - merging previous changes into the staging branch, and further updates to the Person Details page.

Additional information retrieved from the ESB and displayed on the Person Details page:

- Information on issuances linked to cases, such as last issuance date, amount, and type
- Sorting of cases by issuance existence, then date
- Applications for given client in ARIES, information including application number, status, and received date

Other notable changes:
- Addition of explicit ARIES and EIS Client ID search fields on the Search page
- Redirection of a user's session to a log out page after 15 min of inactivity (and addition of log out page)
- Ability to cancel a search
- Display of current user's username (with mocked data)

## `Sprint 6 (2018/04/04 - 2018/04/17)`

This sprint was somewhat limited in development, instead allowing security processes to advance in preparation of access to production data.

Notable changes:

- Review process of code from previous sprints, then merging of this code into staging
- Display of current user's name (with real data)
- UI changes, rewording of labels and addition of 'return to search results' link
- Extensive increase in client side code coverage
- Changes to API, improvement of Person Details performance and changes in error handling
- Changes to search results returned and displayed, error message on results over 50 records, and removal of records without ARIES or EIS ID

## `Sprint 7 (2018/04/18 - 2018/05/01)`

TBD

## `Sprint 8 (2018/05/02 - 2018/05/15)`

TBD