# Database design of Connect

The database of Connect has 44 tables in PostgreSQL 16, in third normal form (3NF). The tables and
how they connect are on the Figma page. This document lists the files, how to start the database,
the choices in the design and the standards it follows.

## Files

| What | Where |
|---|---|
| The diagram (EERD): all tables, columns, types, keys and relations | [Figma, page "Data model (EERD)"](https://www.figma.com/design/SAVxDf3rlsWz22qlzTEYte/Young-Amstelland-Connect--team-workspace?node-id=855-3) |
| The database | [postgres/schema.sql](postgres/schema.sql) |
| Test data for all 44 tables | [postgres/dummydata.sql](postgres/dummydata.sql) |
| Start the database and pgAdmin on your laptop | [postgres/docker-compose.yml](postgres/docker-compose.yml) |

## Run it locally

In the folder `postgres/`:

```bash
docker compose up -d      # start: builds the database and fills it with the test data
docker compose down       # stop: everything is thrown away
```

Open http://localhost:5050 to look inside with pgAdmin. No login needed. Only for use on your own
laptop.

## Design choices

| Choice | Other option | Why this one |
|---|---|---|
| Prices are stored in `product_version` | Copy the name and price onto every order line | A copy breaks 3NF. Without the copy, an old order would show today's price. With a new version for every price change, the order keeps the price that was paid and stays in 3NF. |
| A unique index on the pair of members in `connection_request` | Two extra stored columns `pair_low` and `pair_high` | Both stop a second request between the same two people. The extra columns store data that can be calculated, which breaks 3NF. The index needs no extra columns. |
| Logo and pictures in their own tables (`organisation_logo`, `project_image`, `news_image`) | Three empty-or-filled columns on the owner table | The size of a picture belongs to the picture, not to the owner. In their own table every column is required. |
| `saved_item` is one table, without a foreign key on `item_id` | Three separate tables, one per kind | One query for the Saved page instead of three. Downside: a save can point to something that was deleted, so the page skips those. |
| `login_failure` and `login_lock` use the typed email, not an account id | A link to `account` | A lock must also work for an email without an account. Otherwise you could find out which emails are registered. |
| When an account is deleted: personal data goes, the rest stays without the name | Delete everything, or block the delete | Personal data must go. Paid orders, news and reports must stay for the foundation, only the link to the person is emptied. |
| `uuid` as key, made with `Guid.CreateVersion7()` | A counting number, or a fully random uuid | A counting number lets anyone guess the next member. A version 7 uuid is still hard to guess but is sorted by time, which keeps the index fast. |
| An index on every foreign key, on the columns old rows are cleaned up by, and for searching names | Only the indexes the pages need | Tested with about 4.8 million rows: deleting an account 5.9 ms instead of 94 ms, cleaning up 10.8 ms instead of 255 ms, searching a name 2.4 ms instead of 52 ms. |
| Lists use page numbers | "Give the next 20 after this one" (keyset paging) | Keyset is faster far down a list (0.07 ms against 12.6 ms at page 5000), but the difference is small and the team rule says page numbers. |
| The test database is rebuilt at every start | Keep the data between starts | What you see always matches the files. |
| The Docker file is in `postgres/`, not in the root | A `docker-compose.yml` in the root | The root file is for the team's real setup (tasks 3.2 and 20.2). This one is only for this design. |

## Standards

| Standard | Source | Why it fits |
|---|---|---|
| ERD, then EERD with primary keys, foreign keys and link tables | [HvA, ERD](https://knowledgebase.hbo-ict-hva.nl/1_beroepstaken/software/ontwerpen/relationele_databases/erd/), [HvA, EERD](https://knowledgebase.hbo-ict-hva.nl/1_beroepstaken/software/ontwerpen/relationele_databases/eerd/) | The method of the study. The team builds straight from it. |
| Normalisation to 3NF | [HvA, EERD](https://knowledgebase.hbo-ict-hva.nl/1_beroepstaken/software/ontwerpen/relationele_databases/eerd/) | No data stored twice, so it cannot disagree with itself. |
| Crow's foot notation | [Mermaid, ER diagrams](https://mermaid.js.org/syntax/entityRelationshipDiagram.html) | The notation the team already uses. |
| Checks, foreign keys and an index on each foreign key | [PostgreSQL, Constraints](https://www.postgresql.org/docs/16/ddl-constraints.html) | The database itself refuses wrong data, even if the code has a bug. |
| Index on an expression | [PostgreSQL, Indexes on Expressions](https://www.postgresql.org/docs/16/indexes-expressional.html) | Keeps the pair of members unique without extra columns. |
| Partial index | [PostgreSQL, Partial Indexes](https://www.postgresql.org/docs/16/indexes-partial.html) | An index on only the few rows that are searched stays small. |
| Trigram index | [PostgreSQL, pg_trgm](https://www.postgresql.org/docs/16/pgtrgm.html) | Makes searching a name by a part of it fast. |
| `citext` for email | [PostgreSQL, citext](https://www.postgresql.org/docs/16/citext.html) | `Sam@` and `sam@` count as the same email. |
| `timestamptz` in UTC | [PostgreSQL, Date/Time Types](https://www.postgresql.org/docs/16/datatype-datetime.html) | A time means the same everywhere. |
| Team code conventions | `docs/team/codeConventions.md` | `snake_case` names, `uuid` keys and `citext` email are team rules. |
| EF Core indexes | [Microsoft, EF Core Indexes](https://learn.microsoft.com/en-us/ef/core/modeling/indexes) | The team uses EF Core, which also adds an index on each foreign key. |
| uuid version 7 | [RFC 9562](https://www.rfc-editor.org/rfc/rfc9562), [Microsoft, Guid.CreateVersion7](https://learn.microsoft.com/en-us/dotnet/api/system.guid.createversion7) | A standard uuid that .NET can make in one call. |
| Measuring with `EXPLAIN ANALYZE` | [PostgreSQL, Using EXPLAIN](https://www.postgresql.org/docs/16/using-explain.html), [LIMIT and OFFSET](https://www.postgresql.org/docs/16/queries-limit.html) | Shows how fast a query really is, instead of guessing. |
| Docker Compose, PostgreSQL and pgAdmin images | See the sources below; each setting in `docker-compose.yml` names its source | The whole team starts the same database with one command. |

## Sources

Davis, K., Peabody, B., & Leach, P. (2024). *Universally Unique IDentifiers (UUIDs)* (RFC 9562). RFC Editor. https://www.rfc-editor.org/rfc/rfc9562

Docker. (n.d.-a). *Bind mounts*. Docker Docs. Retrieved October 6, 2026, from https://docs.docker.com/engine/storage/bind-mounts/

Docker. (n.d.-b). *Compose file reference*. Docker Docs. Retrieved October 6, 2026, from https://docs.docker.com/reference/compose-file/

Docker. (n.d.-c). *Control startup and shutdown order in Compose*. Docker Docs. Retrieved October 6, 2026, from https://docs.docker.com/compose/how-tos/startup-order/

Docker. (n.d.-d). *Define services in Docker Compose*. Docker Docs. Retrieved October 6, 2026, from https://docs.docker.com/reference/compose-file/services/

Docker. (n.d.-e). *docker compose up*. Docker Docs. Retrieved October 6, 2026, from https://docs.docker.com/reference/cli/docker/compose/up/

Docker. (n.d.-f). *dpage/pgadmin4* [Docker image]. Docker Hub. Retrieved October 6, 2026, from https://hub.docker.com/r/dpage/pgadmin4

Docker. (n.d.-g). *Networking in Compose*. Docker Docs. Retrieved October 6, 2026, from https://docs.docker.com/compose/how-tos/networking/

Docker. (n.d.-h). *postgres* [Official Docker image]. Docker Hub. Retrieved October 6, 2026, from https://hub.docker.com/_/postgres

Docker. (n.d.-i). *tmpfs mounts*. Docker Docs. Retrieved October 6, 2026, from https://docs.docker.com/engine/storage/tmpfs/

Docker. (n.d.-j). *Version and name top-level elements*. Docker Docs. Retrieved October 6, 2026, from https://docs.docker.com/reference/compose-file/version-and-name/

Hogeschool van Amsterdam. (2024). *Database-ontwerp (EERD)*. HBO-ICT Knowledgebase. https://knowledgebase.hbo-ict-hva.nl/1_beroepstaken/software/ontwerpen/relationele_databases/eerd/

Hogeschool van Amsterdam. (2026). *Entity Relationship Diagram (ERD)*. HBO-ICT Knowledgebase. https://knowledgebase.hbo-ict-hva.nl/1_beroepstaken/software/ontwerpen/relationele_databases/erd/

Mermaid. (n.d.). *Entity Relationship Diagrams*. Retrieved October 6, 2026, from https://mermaid.js.org/syntax/entityRelationshipDiagram.html

Microsoft. (n.d.-a). *Guid.CreateVersion7 method*. Microsoft Learn. Retrieved October 6, 2026, from https://learn.microsoft.com/en-us/dotnet/api/system.guid.createversion7

Microsoft. (n.d.-b). *Indexes* (EF Core). Microsoft Learn. Retrieved October 6, 2026, from https://learn.microsoft.com/en-us/ef/core/modeling/indexes

pgAdmin Development Team. (n.d.-a). *Container deployment*. pgAdmin 4 documentation. Retrieved October 6, 2026, from https://www.pgadmin.org/docs/pgadmin4/latest/container_deployment.html

pgAdmin Development Team. (n.d.-b). *Import/Export Servers*. pgAdmin 4 documentation. Retrieved October 6, 2026, from https://www.pgadmin.org/docs/pgadmin4/latest/import_export_servers.html

pgAdmin Development Team. (n.d.-c). *The config.py file*. pgAdmin 4 documentation. Retrieved October 6, 2026, from https://www.pgadmin.org/docs/pgadmin4/latest/config_py.html

The PostgreSQL Global Development Group. (n.d.). *PostgreSQL 16 documentation*: Constraints; Indexes on Expressions; Partial Indexes; pg_trgm; citext; Date/Time Types; Using EXPLAIN; LIMIT and OFFSET; pg_isready; Trust Authentication; SSL Support. Retrieved October 6, 2026, from https://www.postgresql.org/docs/16/

## AI use declaration

AI was used as a writing support tool while working on this document. We used AI to draft
the formulation of sentences, the structure of wording, readability, and clarity from
content that we had provided, and to generate the diagrams. The content itself, including
our experiences, reflections, learning goals, decisions, self-assessments, and
conclusions, is our own. We reviewed the drafts and decided as a team which text to
include.
