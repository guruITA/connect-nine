# Proof: task 7.2 The address that saves a profile (#321)

Merge request !68, branch `feature/321-create-profile-address`. Issue: 07 Create a profile (#25). Files: `api/Connect.Application/Profiles/CreateProfile/`, `api/Connect.Presentation/Profiles/`, `api/Connect.Presentation/Common/Authentication/`, `api/Connect/Program.cs`, the tests in `Connect.Application.Tests`, `Connect.Presentation.Tests` and `Connect.Tests`, and `docs/architecture/api.md`.
Profile: Full Stack Software Developer, level 3. Portflow: Designing and Realising OR1, OR2, OR4 and OR5.

A point that needs someone else is marked TODO with who and what.

## OR1 Designing and building to a plan

School asks: "You design and implement systematically, justify choices in relation to alternatives, and adapt based on new insights."

1. **Which layer does what.** The endpoint takes the account from the session, takes the spaces off the text and checks the fields. The handler in Application checks that the account has no profile yet and saves it through `IAppDbContext`. The table of task 7.1 refuses a wrong age band and a second profile. Every part has the place and the name it has in Nick Hoebe's account code, because the team agreed on the way Nick builds the .NET code.
2. **The fields and the five answers.** First name and region required and at most 60 characters, an age band from a fixed list of three, a bio of at most 280 characters that may be empty. The answers are 201, 400, 401, 409 and 503, each with a plain sentence for the young person.
3. **The alternatives I weighed.** Checking the fields inside the endpoint lost to a FluentValidation class, because the class is tested on its own and the edit page of #64 can use the same rules. One PUT address for creating and editing lost to POST now and its own address in #64, because a POST that refuses a second profile with 409 says clearly that there is one profile per account.
4. **Log in does not exist yet.** The address is the real one and already sits behind the guard. Only the way it knows the account is temporary: in Development, with a setting switched on, a header names the account. Without the setting, and on every server that is not Development, nobody is logged in and the address answers 401. When log in (#15) adds the session cookie, the header and its handler go and the endpoint stays as it is.
5. **What I changed after the first integration test.** The first run of the endpoint tests failed for every endpoint, also the account ones: `IAppDbContext` was not registered, because nothing used it yet. I registered `ConnectDbContext` as `IAppDbContext` in `api/Connect.Infrastructure/DependencyInjection.cs`.

## OR2 Choosing standards and methods

School asks: "You independently select relevant standards and methods from the field and substantiate why they fit the context."

1. **HTTP status codes.** 201 with a Location for a new profile, 400 for wrong fields, 401 without a login, 409 for a profile that already exists and 503 when the database cannot be reached, as the HTTP standard defines them (RFC 9110).
2. **Problem details.** Every error answer is a ProblemDetails object, and wrong fields a ValidationProblem with one message per field, from the standard Problem Details for HTTP APIs (RFC 9457), as the code conventions ask.
3. **Fixed choices as an allow list.** The age band is accepted only when it is exactly one of the three names. The OWASP Input Validation Cheat Sheet asks for "exact membership in the allowed set, including values submitted from drop-down menus", which is what the age band select of task 7.3 sends.
4. **One message per field.** A form shows each message under its own field, so the young person sees what to change where.
5. **What I took from BoKSA.** The article API implementation asks for a fitting status code for every outcome. Every way a save can end has its own code, from 201 to 503, so the screen of task 7.3 turns each code into one outcome and never has to read text.
6. **What I took from BoKSA.** The article Input validation and secure coding asks for validation on the server with allow lists for format, length and type. The validator checks every field on the server, also when the screen already did, and the age band is only accepted from its allow list of three names.

## OR4 Validating the solution

School asks: "You systematically validate that the solution does what it should, and draw conclusions for improvement."

1. **The tests.** 21 tests of the validator with every limit (60 and 61, 280 and 281, the age bands `1`, `youngadult` and `16` refused), 4 tests of the handler (first save, a second profile refused, an unknown account, the database not reachable) and 10 tests of the endpoint against a real PostgreSQL database: 201 and one row, `" Sam "` saved as `Sam`, 400 per field with nothing saved, 409, two saves at the same moment, 401 without an account, 401 with the setting off, an `accountId` in the body ignored, 503 without error text, and no profile text in the log.
2. **Tests broken on purpose.** Without the rule of 280 characters, three tests went red: `BioOf281Characters_ReturnsTheLengthMessage`, `EveryWrongField_ReturnsOneMessageEach` and `Post_WrongFields_ReturnsOneMessagePerFieldAndSavesNothing`. Without the catch of the unique index, two saves at the same moment gave a server error instead of 409 in 8 runs out of 8. Both were put back and all tests are green again.
3. **Tried by hand.** With the server running in Development, a profile was saved with the temporary header, a second one gave 409 and a request without the header gave 401.
4. **What is not covered yet.** A region with a street and a house number (#26), and a street name without a number, which #26 cannot see either. That is acceptable for now, because the page asks for a town or area and says so. The header of the temporary account is only accepted in Development.
- TODO Wesley: the link to the green pipeline run, once the branch has the pipeline of dev with the tests (now it has the pipeline of Nick's branches).

## OR5 Impact on users and society

School asks: "In your design and implementation, you consciously take into account the broader impact on end users and society, while forming your own ethical judgment."

1. **The minimum of personal data.** A first name, an age band instead of an age, a town or area instead of an address, and a short optional line. No birth date, school, street, phone number or email address.
2. **How a minor is protected.** The account always comes from the session and never from the form, so nobody can make a profile for someone else. The text is not written to the log.
3. **My judgment on the short line.** Free text is where a young person can share more than they mean to, such as a school or a phone number, and other members read it on the page of #30. A filter cannot catch every way to write a school or a number. So I keep the warning right under the field, and I advise the client to let members report a profile (#211), so the foundation sees it (#235) and can have the line removed. I would not hide the short line, because it is the one place where a young person says who they are in their own words.

## TODO

- [ ] Wesley: the link to the green pipeline run above, once the branch has the pipeline of dev.
- [ ] Wesley: after the merge, tick the boxes of #321 with the evidence in a comment.

## AI use

AI was used as a writing support tool while working on this document. We used AI to improve the formulation of sentences, the structure of wording, readability, and clarity of text that we had written. The content itself, including our experiences, reflections, learning goals, decisions, self-assessments, and conclusions, is our own. We reviewed the suggested changes and decided as a team which improvements to include.
