# Research Proposal

Name: Gurpreet Singh
Student ID: 500934143

## Title

Faster import of Dutch election results: saving one row at a time compared with saving in
batches, in a Spring Boot application

## Short Description

In semester 3, my team built an election application with Spring Boot and Vue. It shows the
results of Dutch elections, using the open data of the Kiesraad. The data reaches the database
in two steps. First, separate Java programs read the XML files of the Kiesraad and turn them
into JSON files. Then `ElectionService` reads the JSON files and saves everything in a MySQL
database, where the pages of the application read it.

**The problem.** The second step saves every election, party list, candidate and vote one at a
time with `repository.save()`. The 2021 file alone holds 20,909 candidates. For data whose id
comes from the data itself, the Spring Data JPA documentation says that `save()` first checks
the database and then writes. So I expect two database calls per row. Saving in batches should
be much faster, but it is unclear how much faster, how the difference grows with more data, and
whether batching with JPA comes close to plain SQL. This research measures that instead of
guessing.

**Why this topic.** It follows the feedback of my coach: research a performance problem in one
of my own earlier projects, by comparing solutions in a measurable way. The project ended after
semester 3 and nobody works on it anymore, but it has real code and a large real data set. The
result is useful for any application that loads a lot of data through an ORM. It does not
overlap with my teammate Wesley Oerlemans, who researches reading data in Young Amstelland
Connect. My report is reviewed in Sprint 3.

## Possible Research Questions

**Main question.** How does the time to import the 2021 Dutch election results of the Kiesraad
change for row-by-row saving (A), batched JPA saving (B) and batched SQL saving (C), when the
number of imported candidates increases from 1,000 to all 20,909 candidates of the file?

The three ways are explained in the table under *The three ways I compare*.

**Sub-question 1.** How long does each way take, and how many SQL statements does it send to
the database, at 1,000, 5,000, 10,000 and all 20,909 candidates?

**Sub-question 2.** Do the measured differences match what I expect from the number of database
calls: about two per saved row for A, and about one per batch for B and C?

**Hypothesis.** B and C are much faster than A at every size, and the difference in seconds
grows as the number of candidates grows. C is a bit faster than B, because it skips the extra
work of JPA.

## Possible Research Methods

This is an experiment. I build the three ways of saving and measure them. I only use
documentation to explain the current code and what I expect.

### The three ways I compare

All three ways save exactly the same data in the same tables. Only the way of saving differs.

| Way | How it saves | What I expect per row |
|---|---|---|
| **A** | One row at a time with `repository.save()`. This is the existing code in `ElectionService`. | A SELECT and an INSERT, each in its own transaction |
| **B** | JPA `persist()` in one transaction. Hibernate sends the rows in groups of 50 (`hibernate.jdbc.batch_size`). | Only INSERTs, sent per group |
| **C** | Plain SQL with `JdbcTemplate.batchUpdate()`, without JPA, in groups of 1,000. | Only INSERTs, sent per group |

For B and C, the MySQL driver setting `rewriteBatchedStatements=true` makes a group really go
to the database as one statement.

### How I compare them

I build one test runner. For every way and every size, it does this:

1. Empty the database.
2. Load the part of the 2021 JSON file with the chosen number of candidates.
3. Start the timer.
4. Save the data with way A, B or C.
5. Stop the timer.
6. Write down the time and the number of SQL statements.
7. Check that the data is complete.

Sizes: 1,000, 5,000, 10,000 and all 20,909 candidates, so the last step uses the whole file.
All three ways get exactly the same parts of the file. One candidate means two saved rows: the
candidate and the vote that belongs to it. A part also includes the party lists and contests of
its candidates. So the whole file of 20,909 candidates is about 42,000 rows.

The first run is a warm-up and does not count, because Java is still starting up.
Then every combination runs five times, and I use the middle value (the median).

### What I measure

| What | How |
|---|---|
| Time | `System.nanoTime()` before and after saving |
| Number of SQL statements | A tool between the application and the database (datasource-proxy) counts every statement, the same way for A, B and C |
| Complete data | After every run, `SELECT COUNT(*)` per table. All three ways must store the same number of rows, so no way can win by saving less |

### A fair test

- The same laptop, settings and data for every run, with no heavy programs open.
- MySQL 8 in Docker, because the real application used MySQL. The H2 database of the
  development profile runs inside the application and would hide the database calls.
- Limits I will name in my report: one laptop, one kind of data, and a database on the same
  laptop, so there is less delay than with a database on a real server.

### How I show the results

| Result | Answers |
|---|---|
| A table with the median time per way and size | Sub-question 1 |
| A table with the number of SQL statements per way and size | Sub-questions 1 and 2 |
| How many times faster B and C are than A (time of A divided by time of B or C) | Main question |
| A graph: number of candidates on the x-axis, time on the y-axis, one line per way | Main question |

### Feasibility

The code and the data are already there. On 8 October 2026 I checked that the project still
compiles with Java 21 and that the fields in the 2021 JSON file match the entities. Docker
Desktop is installed. All tools are free. The automatic import was switched off in the last
version of the project, so my first step is to get way A running and measure it once.

## Literature and Sources

Chen, T.-H., Shang, W., Jiang, Z. M., Hassan, A. E., Nasser, M., & Flora, P. (2014). Detecting
performance anti-patterns for applications developed using object-relational mapping. In
*Proceedings of the 36th International Conference on Software Engineering* (pp. 1001–1012).
Association for Computing Machinery. https://doi.org/10.1145/2568225.2568259

A scientific paper that names "one-by-one processing" as a known cause of slow ORM code. Way A
works like this.

Hibernate. (n.d.). *Hibernate ORM 6.5 user guide*. Retrieved October 8, 2026, from
https://docs.jboss.org/hibernate/orm/6.5/userguide/html_single/Hibernate_User_Guide.html

Explains how to save in batches with JPA (`hibernate.jdbc.batch_size`). I use this for way B.
The election project uses Spring Boot 3.3.3, which comes with Hibernate 6.5.

Oracle. (n.d.). *Performance extensions*. MySQL Connector/J developer guide. Retrieved October
8, 2026, from https://dev.mysql.com/doc/connector-j/en/connector-j-connp-props-performance-extensions.html

Explains `rewriteBatchedStatements`, the MySQL setting that makes a batch really go to the
database as fewer statements. I use it for ways B and C.

Spring. (n.d.-a). *JDBC batch operations*. Spring Framework reference documentation. Retrieved
October 8, 2026, from https://docs.spring.io/spring-framework/reference/data-access/jdbc/advanced.html

Explains `JdbcTemplate.batchUpdate()`. I use this for way C.

Spring. (n.d.-b). *Persisting entities*. Spring Data JPA reference documentation. Retrieved
October 8, 2026, from https://docs.spring.io/spring-data/jpa/reference/jpa/entity-persistence.html

Explains why `save()` first checks the database for data whose id comes from the data itself.
This explains why way A is slow.

## AI use declaration

AI was used as a writing support tool while working on this document. I used AI to improve
the formulation of sentences, the structure of wording, readability, and clarity of text that
I had written. The content itself, including my experiences, reflections, learning goals,
decisions, self-assessments, and conclusions, is my own. I reviewed the suggested changes
and decided which improvements to include.
