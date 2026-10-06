# Research & Design Plan of Approach

**Research:** Privacy by Design in the design of Young Amstelland Connect  
**Project:** Young Amstelland Connect  
**Project team:** Connect Nine  
**Client:** WailSalutem Foundation  
**Student:** Gurpreet Singh  
**Version:** 2.0 (second draft, for feedback in sprint 1)  
**Date:** 5 October 2026

## 1. Introduction

This plan describes the research I am carrying out this semester for the Research & Design datapoint. The research addresses how privacy rules work through into the design of Young Amstelland Connect: not only which personal data the platform processes, but also what that means for the screens, texts, default settings and user flows of the website.

The research does not produce a legal judgement. It produces substantiated design requirements, based on official sources and on research with the client and users.

The research is built around the first design of Connect. The team is making that design in Figma now. I derive verifiable criteria from the GDPR, score the first design against them, adjust the design on the points that fall short, build the main adjustments in code, and score the result again with the same criteria. That before and after measurement is what turns the advice into something anyone can check.

## 2. Background

Young Amstelland Connect will be a platform where young people aged 15 to 30 stay visible all year and come into contact with organisations, mentors, events and opportunities. Functions such as registration, profiles, matching, showcases and events require personal data. Because the target group starts at the age of 15, some of the users are minors.

The team has already recognised this in `docs/project/requirements.md`. It contains NFR2, a quality requirement for the whole platform stating that Connect processes personal data of, among others, minors and must therefore comply with the GDPR, regardless of what the client asks for. Because the foundation has no privacy officer, the team writes the privacy rules itself and the client approves them.

NFR2 names what has to happen and which issue it belongs to, but the substantive work is still open. It has not yet been established per functionality which data are necessary, how long they are kept and how the user gets to see this on the screen.

Privacy therefore directly affects the design. Two examples:

- During registration, a user must be able to see what each requested item of data is needed for. That determines the layout and the texts of the registration form.
- After deleting an account, it must be clear what disappears immediately, what is kept temporarily and for how long. That determines the confirmation page, the notification text and the behaviour of the system.

If these choices are made only after the build, then screens, data model and authorisation have to be designed again. That is why I research them in the design phase.

## 3. Problem statement

| W-question | Answer |
|---|---|
| What | It has not yet been researched how privacy rules should work through into the design of Connect. |
| Who | Young people, minor users, parents, organisations, administrators, the client and Connect Nine. |
| Where | In registration, profiles, matching, showcases, events, challenges, account management and the admin dashboard. |
| When | From the moment data are requested, shown, shared, kept or deleted. |
| Why | Unclear or unnecessary data processing harms users, costs trust and leads to rework. |

**Core problem:** Connect Nine has no substantiated design requirements yet with which it can determine, per screen and per function, which data are needed, what the user gets to see about this, which default settings apply and how long data are kept.

## 4. Objective

Before the data-processing functions are built, the research delivers a substantiated recommendation consisting of:

1. an overview of the personal data per functionality, with purpose and necessity;
2. retention periods and deletion rules;
3. concrete design requirements per screen, including information texts and default settings;
4. a weighing of alternatives with their consequences;
5. input for the data model and the authorisation of Connect Nine.

## 5. Research questions

**Main question**

> To what extent does the first design of Young Amstelland Connect meet the privacy requirements that follow from the GDPR, and which design changes produce the largest measurable improvement, given a target group of 15 to 30 year olds that also includes minors?

**Sub-questions**

1. Which personal data does Connect process per functionality and for what purpose?
2. Which requirements do the GDPR and relevant guidelines place on these processing activities, with particular attention to minors?
3. Which data are necessary and which can be dropped, made optional or derived?
4. Which retention periods and deletion rules apply per type of data, and what happens after an account is deleted?
5. How should the user be informed on the screen and which choices should they be able to make, for example during registration, profile visibility and deletion?
6. Which default settings for visibility and contact suit the target group, and what alternatives are there?
7. How are the findings translated into verifiable design requirements that are understandable for the client and feasible for Connect Nine?
8. How does the score of the screens of Connect on those criteria change between the first design and the adjusted design, and which criteria turn out to be the hardest to meet?

## 6. Scope

**Within the scope:** personal data in the core functionalities, data minimisation, retention periods, deletion, the information provided on the screens, default settings for visibility and contact, additional protection of users under 18, and the translation into design requirements, user flows and data model.

**Outside the scope:** binding legal advice, a full Data Protection Impact Assessment, penetration testing and the technical threat model (these belong to the security tasks of Kelvin van Voorst), general web accessibility and WCAG, and legislation on payments.

Accessibility for users with low vision, for example, is important, but belongs to WCAG and not to the GDPR. It stays outside the scope here in order to keep the research clearly bounded.

## 7. Stakeholders

| Stakeholder | Interest | Role in the research |
|---|---|---|
| Young people aged 15 to 30 | Understandable choices and control over their own data | Interviews and review of designs |
| Minor users | Protection against unwanted visibility and contact | Have scenarios reviewed, participation only under conditions |
| WailSalutem Foundation | Trust, workable administration | Validate goals and recommendation |
| Organisations | Reach young people without unnecessary access to data | Interviews about the access they need |
| Administrators | Be able to handle reports and privacy requests | Interviews about working processes |
| Connect Nine | Feasibility of the design requirements | Assessment of technical feasibility |

## 8. Research method

The research is qualitative. I research rules and standards through desk research, wishes and working processes through interviews, and I test design choices with scenarios and prototypes. A survey is not a main method at this stage, because respondents cannot determine what is legally or ethically sound; later on, a survey can measure the preferences of a larger group of young people.

| Sub-question | Method | Source or respondent | Result |
|---|---|---|---|
| 1 | Document analysis, interview | Client brief, requirements, page documents, client | Data overview per function |
| 2 | Desk research, expert review | GDPR, Dutch Data Protection Authority, EDPB | Framework of requirements and open questions |
| 3 | Analysis, interviews | Data overview, client, team | Recommendation per item of data |
| 4 | Desk research, process interview | Official sources, administrators | Retention and deletion rules |
| 5 | Scenario analysis, prototype, user test | Young people, client | Screen requirements and information texts |
| 6 | Analysis of alternatives, user test | Young people, organisations, team | Substantiated choice of default settings |
| 7 | Requirements analysis, validation session | All results, client, team | Design requirements and acceptance criteria |
| 8 | Experiment, a before and after measurement | The first Figma design, the adjusted design | Score per screen before and after, with the criteria that still fail |

### The experiment

Sub-question 8 is answered with a measurement that is applied twice to the same screens:

1. **The instrument.** The criteria from sub-questions 2 to 6 are written as statements that can be answered with yes, no or partly, each with a scoring rule, for example: the screen states per required field why the data is needed. The checklist is fixed before anything is measured, so it cannot be bent towards the design I want to make.
2. **Baseline.** The first design of Connect in Figma is scored per screen and per criterion, before any change.
3. **Adjustment.** The screens are reworked on the points that scored no or partly, and the main adjustments are built in React with TypeScript, so the design is shown to work and not only to look right.
4. **Second measurement.** The adjusted design is scored with the same checklist, by the same rules.

To check that the checklist is unambiguous rather than a matter of opinion, a second person scores a sample of the screens independently. Where the two scores differ, the criterion is too vague and is rewritten. The checklist, the scores and the screens are kept in the repository, so another student can repeat the assessment and reach the same outcome.

## 9. Deliverables

1. The measuring instrument: a checklist of verifiable design criteria derived from the GDPR, with a scoring rule per criterion.
2. The adjusted design in Figma, with the main adjustments implemented in React, shown with code fragments and screenshots.
3. The two measurements: the score of the first design and of the adjusted design, per screen and per criterion.
4. Data overview per functionality, with purpose and necessity.
5. Retention and deletion matrix, including what happens after account deletion.
6. Roles and permissions matrix: who sees which data.
7. Design requirements per screen, with acceptance criteria.
8. Conceptual data model for the client and design input for the EERD of Connect Nine.
9. Advisory report with alternatives, risks and a recommendation.
10. Validation report with the feedback received and the changes made.

A design requirement is only finished when it is verifiable. Examples of the form the research has to deliver:

- The registration form shows per field what the data item is needed for and whether it is mandatory.
- The confirmation page for account deletion states what disappears immediately, what is kept temporarily and for how long.
- A new profile is set to the most protected visibility by default; the user widens this themselves.

The content of these requirements follows from the research and is not fixed yet.

## 10. Planning

| Period | Activities | Result |
|---|---|---|
| Sprint 1 (21 Sep to 9 Oct 2026) | Define the topic, determine research questions and method, collect first sources, align with team and lecturer | This plan of approach and the feedback received |
| Sprint 2 (19 Oct to 6 Nov 2026) | Desk research, document analysis, inventory of data, conduct interviews, derive and fix the checklist, score the first Figma design | Research notes, the measuring instrument and the baseline measurement |
| Sprint 3 (9 to 27 Nov 2026) | Adjust the design, implement the main changes, second measurement, analyse, validate with stakeholders, write the report and hand it to the reviewer around 20 November, process the peer review | The adjusted design, the implementation and the R&D report |
| Sprint 4 (30 Nov to 18 Dec 2026) | Process feedback, hand the criteria to the team as a reusable check for new screens | Final recommendation |
| Sprint 5 (4 to 8 Jan 2027) | Finish and hand over | Transferable documentation |

The checklist has to be finished and fixed before the first design is scored. If the Figma design is not ready at the start of sprint 2, the baseline measurement runs on the screens that do exist, and that is reported.

I record changes to the planning together with the reason for them.

## 11. Quality, reliability and ethics

I base legal statements on official sources; blogs and AI output do not count as a source. I keep facts, assumptions and client choices separate. I summarise interviews and submit them to the respondent for checking. I compare alternatives using the same criteria, and every design requirement is traceable to a source or a research result.

Because the research itself collects data from respondents, I collect only what is needed, ask for consent beforehand, anonymise notes, do not keep recordings longer than necessary, and a respondent can always stop. I only carry out research with minors if consent and supervision are properly arranged; otherwise I work with adult representatives.

## 12. Risks

| Risk | Control measure |
|---|---|
| The topic becomes too broad | Scope limited to the core functionalities and the translation into design |
| Insufficient access to young people | Find respondents in good time, discuss alternatives with client and lecturer |
| Legal sources interpreted incorrectly | Use official sources, have assumptions reviewed |
| The client's wishes conflict with privacy rules | Make alternatives and consequences explicit and give a reasoned recommendation |
| Overlap with the security tasks within the team | Align with Kelvin van Voorst (issues 12, 13 and 19) |
| A design requirement is not feasible within the project | Test feasibility with Connect Nine during the research |
| The first Figma design is not ready in time for the baseline measurement | Agree the date with the team in sprint 1, and measure the screens that exist |
| The checklist is bent towards the design I want to make | Write and fix the checklist before the baseline measurement, and keep both versions in the repository |
| I am assessing my own adjustment, so the result is flattering by definition | Report the second measurement per criterion, including the criteria that still fail, and have a sample scored by someone else |

## 13. Relation to the assessment criteria

This research is evidence for Analysing & Advising at level 3.

| Criterion | Evidence |
|---|---|
| AA1 | This plan, the substantiation of the method and the recorded adjustments |
| AA2 | Source assessment, separation of facts and assumptions, verified use of AI |
| AA3 | Analysis from the perspective of young people, minors, organisations, administrators and the client |
| AA4 | Weighing of alternatives with risks and a substantiated recommendation |
| AA5 | An understandable recommendation for the client alongside technical design input for the team |

The design requirements and flows can later contribute to Design & Realisation, once they have been validated and applied.

## 14. Possible sources

This is a provisional selection of sources that I am likely to use, ordered by the sub-question they are intended for. The list is not fixed yet. During the research it may turn out that a source is less relevant than expected, or that a topic calls for a source that is not listed here. Which sources I actually used will be stated in the final reference list of the research report.

The references follow APA 7. While working them out, I check the current version of each source and note the date of retrieval.

**Legislation (sub-question 2)**

1. European Union. (2016). *Regulation (EU) 2016/679 of the European Parliament and of the Council on the protection of natural persons with regard to the processing of personal data (General Data Protection Regulation)*. EUR-Lex. https://eur-lex.europa.eu/eli/reg/2016/679/oj

For this research, Article 5 (principles), Article 8 (consent of children), Articles 12 to 14 (duty to inform), Article 17 (right to erasure) and Article 25 (data protection by design and by default) are particularly relevant.

**Guidance from the supervisory authority (sub-questions 1 to 5)**

2. Autoriteit Persoonsgegevens. (n.d.). *Wat zijn persoonsgegevens?* [What are personal data?]. https://www.autoriteitpersoonsgegevens.nl/themas/basis-avg/privacy-en-persoonsgegevens/wat-zijn-persoonsgegevens
3. Autoriteit Persoonsgegevens. (n.d.). *Grondslagen AVG uitgelegd* [GDPR legal bases explained]. https://www.autoriteitpersoonsgegevens.nl/themas/basis-avg/avg-algemeen/grondslagen-avg-uitgelegd
4. Autoriteit Persoonsgegevens. (n.d.). *Grondslag toestemming* [Legal basis of consent]. https://www.autoriteitpersoonsgegevens.nl/themas/basis-avg/avg-algemeen/grondslag-toestemming
5. Autoriteit Persoonsgegevens. (n.d.). *Bewaren van persoonsgegevens* [Retention of personal data]. https://www.autoriteitpersoonsgegevens.nl/themas/basis-avg/privacy-en-persoonsgegevens/bewaren-van-persoonsgegevens
6. Autoriteit Persoonsgegevens. (n.d.). *Recht op gegevens verwijderen* [Right to erasure of data]. https://www.autoriteitpersoonsgegevens.nl/themas/basis-avg/privacyrechten-avg/recht-op-gegevens-verwijderen
7. Autoriteit Persoonsgegevens. (n.d.). *Recht op informatie* [Right to information]. https://www.autoriteitpersoonsgegevens.nl/themas/basis-avg/privacyrechten-avg/recht-op-informatie
8. Autoriteit Persoonsgegevens. (n.d.). *Toegang tot persoonsgegevens* [Access to personal data]. https://www.autoriteitpersoonsgegevens.nl/themas/beveiliging/beveiliging-van-persoonsgegevens/toegang-tot-persoonsgegevens

Source 4 covers the rule that for users under the age of 16, consent from a parent or guardian is required. That directly affects the registration flow of Connect.

**European guidelines (sub-questions 2, 5 and 6)**

9. European Data Protection Board. (2020). *Guidelines 4/2019 on Article 25: Data protection by design and by default* (version 2.0). https://www.edpb.europa.eu/our-work-tools/our-documents/guidelines/guidelines-42019-article-25-data-protection-design-and_en
10. European Data Protection Board. (2020). *Guidelines 05/2020 on consent under Regulation 2016/679* (version 1.1). https://www.edpb.europa.eu/our-work-tools/our-documents/guidelines/guidelines-052020-consent-under-regulation-2016679_en

Source 9 is the key source for this research, because it describes how data protection must be built into the design and into the default settings.

**Technical standards (sub-questions 4 and 5)**

11. OWASP Foundation. (n.d.). *User privacy protection cheat sheet*. OWASP Cheat Sheet Series. https://cheatsheetseries.owasp.org/cheatsheets/User_Privacy_Protection_Cheat_Sheet.html
12. OWASP Foundation. (n.d.). *Application Security Verification Standard*. https://owasp.org/www-project-application-security-verification-standard/

Source 11 translates privacy principles into concrete measures in an application and therefore connects to the design requirements. I use source 12 only for the requirements that concern the retention and deletion of data; the broader security track belongs to Kelvin van Voorst.

**Internal project documents (sub-question 1)**

These documents are the material for the document analysis: from them I take which data Connect wants to process per functionality. They are not public and are kept in the team's GitLab repository.

13. Connect Nine. (2026). *clientBrief.md* [Internal project documentation]. `docs/client/clientBrief.md`.
14. Connect Nine. (2026). *requirements.md* [Internal project documentation]. `docs/project/requirements.md`.
15. Connect Nine. (2026). *Page documents of Connect* [Internal project documentation]. `docs/pages/`.

**Academic literature still to be found**

For the theoretical framework I search through the HvA library, Google Scholar and ordinary searches on Google. I use terms such as *privacy by design*, *children's privacy online platforms*, *data minimisation*, *privacy defaults* and *consent design for minors*.

I mainly use Google to quickly see which organisations, guidelines and examples exist on a topic, and to click through to the original source. What I find there I do not simply adopt: a blog, a news item or a summary by a law firm I use at most as a finding aid, not as a source. I check the statement itself against the official source, such as the legal text, the Dutch Data Protection Authority or an academic article. Only the latter do I include with a full APA reference in the research report.

## 15. AI use declaration

AI was used as a writing aid in structuring and formulating this plan. The research question, the methodological choices, the assessment of sources, the interviews, the analysis and the conclusions are my own work and my own responsibility. AI output does not count as a legal or academic source; I check important statements against official sources.
