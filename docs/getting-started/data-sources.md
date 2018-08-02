# Data Sources

The system currently retrieves data from three benefits systems and provides a unified view of that information for field workers.

Some of the content below is restated from the [Prototype Findings Document](https://github.com/AlaskaDHSS/RFP-Search-Unification/blob/master/4-Prototype-Findings.md).

### Master Client Index (MCI)

The Master Client Index contains high-level information about all individuals participating in Alaska benefits programs. Most searches will start here using basic criteria such as participant name, social security number, or client ID numbers. The client IDs are references to records in other benefits systems such as EIS and ARIES.

### Alaska's Resource for Integrated Eligibility Services (ARIES)

Alaska’s Resource for Integrated Eligibility Services (ARIES) is a solution implemented by a previous vendor to support eligibility determination for MAGI Medicaid. This is a Java-based solution with a Postgres backend running in a managed hosting environment in the State of Alaska.

### Eligibility Information System (EIS)

The Eligibility Information System (EIS) is a legacy system that was implemented prior to the development of ARIES and contains applicant information for public assistance programs and services. This is a Natural/ADABAS system running on the state mainframe.

Because of some limitations with the ARIES system, eligibility technicians occasionally “fall back” to EIS to process applications. As such, MAGI Medicaid applications are contained in both ARIES and EIS systems.