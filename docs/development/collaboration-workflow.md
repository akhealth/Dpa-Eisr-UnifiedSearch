# Collaboration Workflow
This page provides an explanation of communication and development processes used on the project.

## Communication
Since this project involves a large team with members from different remote organizations, various systems and strategies are employed to facilitate effective communication.
* Every day, team members report in the daily standup on what they did, what they plan to do, and if there is anything blocking their work. These meetings are held on Cisco's WebEx to allow remote participation.
* The easiest way to keep track of communication and reach team members is through the project's Slack channel.

## Development
#### Sprints

The project is organized by 2 week sprints, where every sprint, a substantial part of the product is delivered to the user for the user to accept. Each sprint represents a new stage of development, and the previous sprint is evaluated to account for lessons learned and improve processes. Three things are evaluated: what went well, what can be improved, and what did not work.

#### VSTS
 Work is centered around the team's Visual Studio Team Services project. Stories are created from interacting with users to generate requirements. These are put in cards in VSTS which can be taken for work by developers and UI/UX designers to turn into product features. 

#### Code
*TODO: Specify development/staging/master process*

After features are completed, developers create pull requests to merge each feature into the **development** branch. After 2 reviewers accept the pull request, it is merged. Periodically (generally every sprint), a pull request is created to merge the changes from **development** into **staging**. This pull request is reviewed by developers from the State of Alaska and 18F, feedback is given to and incorporated by the Resource Data developers, and once all problems are resolved, the PR is completed. 

After it is completely tested, **staging** is merged into **master**.

The Resource Data team is primarily responsible for developing the application and the API for accessing the database. Developers from the State of Alaska provide the Resource Data team with needed data.

#### Estimation
The team currently uses the 'Easy Estimations' tool in VSTS, located under the Backlogs->Board view, using a points-for-effort system.

We previously used planning poker and a points-for-effort system to estimate the effort of tasks. (1 point is 4 hours, 2 is 8 hours, etc.) 

We are considering using Michael Lant's 2-vector approach in the future. Additional reading can be found here: 
https://michaellant.com/2010/07/05/estimating-effort-for-your-agile-stories-2/