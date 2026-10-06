# Research proposal: Google OAuth and registration friction

**Project:** Young Amstelland Connect  
**Research area:** OAuth authentication  
**Related issue:** Board issue 106, Research and development  
**Status:** Research proposal draft  
**Date:** 6 October 2026

## 1. Problem statement

Young Amstelland Connect is a standalone community and talent platform for young
people and young professionals from Amstelland aged 15 to 30. The platform should
connect users with organisations, mentors, opportunities, events and community
activities. Before users can use those functions, they must create an account.

Traditional email-password registration requires users to enter information
manually and create another password. This may create friction for a target group
that already uses external online accounts. A long or inconvenient registration
process can cause users to abandon registration before creating a profile or
discovering an opportunity. It can also create password-reset and account-support
work for the foundation.

Google OAuth/OpenID Connect could simplify registration by allowing users to
authenticate with an account they already use. This may reduce the number of
manual fields and the time needed to create an account, but it may also introduce
provider dependency, privacy concerns and reluctance to use an external account.
It is not yet known whether Google social login actually improves the registration
experience for Connect users.

This research compares Google social login with email-password registration in a
small Connect prototype. The result will help the team determine whether OAuth is
a useful registration option for the product. The experiment does not use the
production YAA platform, real Connect users or real personal data from young
people.

## 2. Business perspective

The client brief describes Connect as an ecosystem in which young talent can
become visible and find people, organisations, opportunities and challenges all
year round. Registration is therefore not the final goal: it is the first step
towards profile creation, discovery, participation and community interaction.

The business hypothesis is:

> If registration requires less manual work and takes less time, more visitors
> may complete registration and reach their first meaningful action in Connect.

OAuth may contribute to this by:

- reducing the number of fields users must enter;
- removing the need to create and remember another password;
- shortening the path from first visit to a Connect profile;
- reducing failed registrations caused by password requirements;
- reducing password-reset and account-support work;
- making it easier for users to return to the platform.

These benefits are hypotheses, not assumptions that the experiment may present as
facts. Google login may be less attractive to users who do not want to share
identity data, do not have a Google account or prefer a separate Connect account.
The provider must therefore be assessed on both user convenience and business
fit.

The research covers the complete intended audience of 15 to 30, but the initial
experiment recruits adults. Participants aged 15 to 17 are discussed as an
important target-group limitation and may only be added later after a proper
consent and supervision process has been arranged.

## 3. Objective

The research delivers a measured comparison of two registration variants and a
recommendation about whether Google social login should be offered as a
registration option in Connect.

The research should produce:

1. a short explanation of OAuth 2.0, OpenID Connect and Google sign-in;
2. a business-focused comparison of social login and email-password registration;
3. a React/Vite and .NET Minimal API prototype with both registration variants;
4. measurements of registration completion, completion time, required fields and
   participant preference;
5. a recommendation with limitations and implementation requirements.

## 4. Research question

### Main question

> To what extent does Google OAuth registration reduce registration friction
> compared with email-password registration for a Young Amstelland Connect
> prototype?

### Sub-questions

1. How do Google OAuth and email-password registration differ in completion time,
   required input and registration completion rate?
2. Which registration method do adult test participants prefer after trying both
   variants, and why?

## 5. Scope

### Within scope

- Google as the OAuth/OIDC provider for the prototype;
- social login as an alternative to email-password registration;
- users aged 15 to 30 as the product target group;
- adult participants aged 18 to 30 for the initial experiment;
- the React with Vite frontend;
- the .NET Minimal API backend;
- a controlled Google test account;
- anonymous measurements;
- registration completion, time, field count and user preference;
- business implications for onboarding and participation;
- privacy implications of requesting provider identity data.

### Outside scope

- changing the existing YAA production platform;
- integrating Connect authentication with YAA;
- administrator authentication;
- account linking between multiple providers;
- implementing every provider in the prototype;
- a full provider decision for Facebook, Instagram, Snapchat, Apple and
  Microsoft;
- penetration testing or a complete security audit;
- a complete GDPR assessment or binding legal advice;
- testing with real data from young people;
- measuring long-term retention or community participation;
- building a production-ready identity platform.

Google is the only provider implemented in the experiment. The other providers
may be compared briefly as background context, but their popularity among young
people does not by itself make them suitable authentication providers.

## 6. Technical context and assumptions

Connect is a standalone application with its own technical environment and
database. The existing YAA platform remains separate and continues to handle
the official award cycle. The OAuth experiment must preserve this separation.

The prototype assumptions are:

- frontend: React with Vite;
- backend: .NET Minimal APIs;
- provider: Google;
- identity data: provider ID, name and email;
- account linking: not supported;
- administrator access: separate authentication method;
- test data: invented or controlled test-account data only.

The implementation example should use the Authorization Code Flow with PKCE or
the standard secure flow supported by the selected .NET integration. Provider
tokens should not be exposed to the React application unnecessarily. The
prototype is an experiment and not a production security review.

## 7. Stakeholders and business interests

| Stakeholder | Interest in the result |
|---|---|
| Young people and young professionals aged 15–30 | Fast, understandable and trustworthy registration |
| Users aged 15–17 | Access to Connect with appropriate privacy and consent safeguards |
| Organisations and companies | A usable way to create an organisation account and participate |
| Mentors, experts and ambassadors | Low-friction access to opportunities to support young people |
| WailSalutem Foundation | More participation without unnecessary support burden or provider risk |
| Connect Nine | A feasible authentication flow for the existing stack |
| Administrators | A separate and stronger authentication route |
| HvA teacher | A small, measurable and reproducible R&D experiment |

## 8. Research method

The research uses a controlled prototype experiment. Each participant performs
the same registration task with both variants:

1. email-password registration;
2. Google social login registration.

The order of the variants should be varied where practical, because using one
flow first may make the second flow easier through familiarity. The prototype,
device, task description and success definition should remain the same.

### Participants

The initial experiment uses adult volunteers, preferably adults from the
intended 18–30 audience. Participants aged 15–17 are not recruited unless the
team first arranges:

- informed consent from a parent or legal guardian;
- assent from the minor participant;
- appropriate supervision;
- a clear plan approved by the responsible school or research contact.

If this process is not arranged, minors remain part of the target-group
discussion and limitation section only. If enough minors are later recruited,
their results are analysed separately from the adult results.

### Data collection

Only anonymous measurements are recorded:

- participant code, if needed for ordering measurements;
- registration variant;
- whether registration was completed;
- completion time;
- number of required manual fields;
- chosen preferred variant;
- short reason for the preference;
- relevant observation, without identifying information.

The research does not record names, email addresses, Google account details or
provider tokens. The Google test account is controlled by the team and is not
used as a source of participant data.

### Measurements

| Measurement | Operational definition |
|---|---|
| Completion rate | Percentage of attempts that reach the defined successful-account state |
| Completion time | Time from the start instruction until the successful-account state |
| Required fields | Number of fields the participant must manually complete |
| User preference | Variant selected after both tasks, with a short reason |

The successful-account state must be defined before testing, for example:
the prototype confirms that a Connect account was created and shows the first
authenticated screen.

### Analysis

For each variant, the research reports:

- number of completed and abandoned attempts;
- completion percentage;
- average and median completion time where the sample permits;
- number of required manual fields;
- preference counts and recurring reasons.

The results should not be presented as statistically representative of all
15–30-year-olds. They are an experiment with a limited convenience sample. The
report should distinguish measured results, participant opinions and business
interpretation.

## 9. Provider context

The broader research may compare Google, Microsoft, Facebook, Instagram,
Snapchat and Apple. The comparison should assess:

- whether the provider offers a supported authentication flow;
- whether it provides stable identity information;
- whether it is relevant to the different Connect user groups;
- what data and permissions are requested;
- documentation and .NET implementation effort;
- provider dependency and availability risk;
- privacy and account-recovery implications.

Instagram and Snapchat require particular care. Their popularity with young
people does not guarantee that they are suitable general-purpose identity
providers. The research should distinguish popularity from actual OAuth/OIDC
availability and technical suitability.

## 10. Privacy, security and ethics

OAuth authentication does not remove the need to make independent privacy
decisions. Connect should request only the identity data needed to create an
account: provider ID, name and email, subject to validation of the final
requirements.

The experiment:

- uses a controlled provider test account;
- uses invented or test data;
- does not use production data;
- records anonymous measurements only;
- does not recruit minors without consent and supervision;
- keeps the adult-only experiment as the default plan;
- does not treat Google as a source of age verification or parental consent.

The final implementation must also consider state validation, redirect URI
validation, token and session handling, logout, provider failure and account
creation errors. These are implementation requirements and risks, not separate
measurements in this small study.

## 11. Expected recommendation

The recommendation will be based on the experiment rather than on the
assumption that OAuth is automatically better. Google social login is a
candidate for an additional registration option if it:

- measurably reduces registration friction;
- is acceptable to participants;
- provides sufficient identity data;
- can be implemented safely in the existing stack;
- does not create unacceptable privacy or provider-dependency risks.

Email-password registration should remain a possible fallback unless the
research produces a justified reason not to offer it. The separate
administrator authentication method remains outside this experiment.

## 12. Limitations and risks

| Risk or limitation | Control measure |
|---|---|
| Adult participants do not represent all users aged 15–30 | State the limitation and analyse any minor results separately |
| The sample is too small for broad claims | Report descriptive results, not population-wide conclusions |
| Participants learn from the first registration flow | Vary the order where practical |
| A test Google account is unlike a normal user account | Record this as a prototype limitation |
| OAuth appears faster because it asks for less data | Compare required data and account state explicitly |
| Provider changes its API or policy | Record provider documentation version and retrieval date |
| Participants reveal personal data during testing | Use controlled accounts and anonymous records |
| The topic grows into a full authentication audit | Keep security, provider comparison and implementation bounded |
| Minor participation is not ethically organised in time | Do not recruit minors; retain the limitation statement |

## 13. Relation to issue 106

This is a research topic rather than a descriptive exploration because it
builds two registration variants and measures how they compare on defined
outcomes.

The proposal contains the five required elements:

1. title;
2. problem statement;
3. main question and two sub-questions;
4. planned method;
5. initial sources.

The experiment uses a small React/.NET prototype and invented or controlled
data. It does not use real data of young people or change the production YAA
platform.

## 14. Initial sources

These sources are a starting point and must be checked for the required
academic quality and current versions before the proposal is submitted.

1. OpenID Foundation. (n.d.). *OpenID Connect Core 1.0 incorporating errata set 1*.
   https://openid.net/specs/openid-connect-core-1_0.html

2. Hardt, D. (Ed.). (2012). *The OAuth 2.0 authorization framework* (RFC 6749).
   Internet Engineering Task Force. https://www.rfc-editor.org/rfc/rfc6749

3. Google. (n.d.). *OpenID Connect*. Google Identity.
   https://developers.google.com/identity/openid-connect/openid-connect

The final version should add or substitute a peer-reviewed source about
authentication usability, registration friction or user behaviour if the HvA
topic rubric requires an academic source. Official technical documentation is
used for implementation facts; it is not treated as evidence that OAuth will
improve business outcomes.

## 15. AI use declaration

AI was used to help structure and formulate this research proposal. The
research question, experimental decisions, participant safeguards, analysis,
source assessment and conclusions remain the responsibility of the student.
AI output is not treated as a scientific, legal or technical source without
verification against the original source.

