# Research Proposal

Name: Wesley Oerlemans
Student ID: 500926596

## Title

Keeping the profile endpoints of Young Amstelland Connect fast as related data, data volume
and users grow

## Short Description

Our team builds Young Amstelland Connect, a platform for the WailSalutem Foundation where
users aged 15 to 30 find and connect with other users, organizations and opportunities. The
backend runs on ASP.NET Core with Entity Framework Core for data access, against PostgreSQL
16. A straightforward Entity Framework Core implementation can send one query for a profile
and one more query for every related record, the N+1 pattern, and I expect that pattern to
slow the profile endpoints down as related records, profiles and users grow. The requirements
document names a responsive platform as NFR4, but sets no numeric target, so nobody can say
yet when a profile endpoint is fast enough. This matters to the people who rely on Connect:
young people who browse it, jury members and mentors who read profiles, and the
municipalities, schools and companies the WailSalutem Foundation wants to reach. My research
measures this instead of guessing. I build the profile detail and profile list endpoints in
four data loading variants and measure their query counts and response times over defined
ranges of related records, profiles and concurrent users. This is a design question tested by
experiment, not a comparison of two fixed options or an open what-is question. I spend 4
hours a week on it, and my report is reviewed in sprint 3.

## Possible Research Questions

- Main question: How can the profile endpoints of Young Amstelland Connect, built with
  ASP.NET Core and Entity Framework Core on PostgreSQL 16, be designed so that their number of
  SQL queries stays constant and their response time stays within a defined user-facing budget
  as the number of related records per profile, the number of profiles in the database and the
  number of concurrent users grow?
- Sub-question 1: To what extent do the number of SQL queries and the response time of a
  straightforward implementation of the profile detail endpoint and the profile list endpoint
  grow across these ranges, and does that growth show the N+1 pattern (one query plus one
  extra query per related record) that Entity Framework Core's documentation warns about?
- Hypothesis (H1): the lazy baseline's query count rises with every related record step on the
  detail endpoint and with the page size on the list endpoint, and its p95 rises with it.
- Sub-question 2: Which of Entity Framework Core's data loading techniques, eager loading with
  Include, split queries with AsSplitQuery, and projection with Select into a data transfer
  object, if any, keep the query count of both endpoints constant across the same ranges?
- Hypothesis (H2): Include, AsSplitQuery and projection each keep the query count the same at
  every related record step.
- Sub-question 3: Which of those techniques, if any, keeps the response time within the 0.1
  second and 1.0 second limits and within the proposed 200 millisecond budget at each
  concurrency level, and at which combination of related records, profiles and concurrent
  users does each technique first cross each limit, if it crosses it at all?
- Hypothesis (H3): at 50 concurrent users in the core set, projection holds p95 within the
  200 millisecond budget longest and Include crosses it first.

## Possible Research Methods

- A quantitative experiment with desk research for the baseline, not field research.
- Sub-question 1: desk research, then experiment. I read the query log and the k6 p50 and p95
  of the lazy baseline across all ranges, core set first.
- Sub-question 2: experiment. I read the query log of Include, AsSplitQuery and projection
  across the same ranges.
- Sub-question 3: experiment. I measure k6 p50 and p95 per variant at each concurrency level,
  and find the crossing point per limit for each variant that crosses one.
- Validity controls: I control for warm-up and JIT compilation, database and connection pool
  caching, background noise, Docker overhead, synthetic data shape, the load generator's own
  resource use and run-to-run variance, with three runs per cell and the same seed, machine and
  settings across all four variants.

## Literature and Sources

Chen, T.-H., Shang, W., Jiang, Z. M., Hassan, A. E., Nasser, M., & Flora, P. (2014). Detecting performance anti-patterns for applications developed using object-relational mapping. In *Proceedings of the 36th International Conference on Software Engineering* (pp. 1001–1012). Association for Computing Machinery. https://doi.org/10.1145/2568225.2568259

Names the ORM performance anti-patterns, excessive data and one-by-one processing, that my
hypotheses rest on.

Chen, T.-H., Shang, W., Jiang, Z. M., Hassan, A. E., Nasser, M., & Flora, P. (2016). Finding and evaluating the performance impact of redundant data access for applications that are developed using object-relational mapping frameworks. *IEEE Transactions on Software Engineering, 42*(12), 1148–1161. https://doi.org/10.1109/TSE.2016.2553039

Measured a response time gain of 17% on average from resolving redundant data access, the
finding I read my own response time results against.

Microsoft. (n.d.). *Lazy loading of related data*. Microsoft Learn. Retrieved October 5, 2026, from https://learn.microsoft.com/en-us/ef/core/querying/related-data/lazy

Documents the lazy loading technique and its N+1 risk, which my baseline variant tests
directly.

The full method, validity table, planning and further sources are in the complete document,
*Research plan: keeping the profile endpoints of Young Amstelland Connect fast as related
data, data volume and users grow*.

## AI use declaration

AI was used as a writing support tool while working on this document. I used AI to draft
the formulation of sentences, the structure of wording, readability, and clarity from
content that I had provided. The content itself, including my experiences, reflections,
learning goals, decisions, self-assessments, and conclusions, is my own. I reviewed the
drafts and decided which text to include.
