# Young Amstelland Connect, the client's brief

What the client sent about the product, kept verbatim. Received from Rachid Kherrazi,
WailSalutem Foundation, on 22 September 2026. The drawn concept is
`docs/client/clientBriefConcept.jpeg`. The requirements that follow from it are in
`docs/project/requirements.md`, and the open questions for the client are in
`docs/client/clientQuestions.md`.

The studio's own assignment form is "Young Amstelland Award Engaing Social App Form Sem 1-
Social Impact Apps - description sem1-2026-2027".

The client's text is Dutch and is quoted verbatim, so its punctuation stays as the client wrote
it. No wording below was drafted, reworded, or added by AI.

## The text

> ## Young Amstelland Connect
>
> De **Young Amstelland Awards (YAA)** geven jongeren en jonge professionals van 15–30 jaar
> uit Amstelland een podium voor hun talent, maatschappelijke impact, ondernemerschap en
> doorzettingsvermogen. De huidige YAA-website ondersteunt de officiële awardcyclus, waaronder
> nominaties, stemmen, juryproces en het gala.
>
> ### De opdracht
>
> Ontwikkel **Young Amstelland Connect**: een zelfstandig digitaal community- en
> talentplatform dat voortbouwt op YAA, maar **technisch en functioneel losstaat van de
> bestaande YAA-website**.
>
> Deze scheiding is bewust gekozen. De bestaande YAA-website blijft het officiële en
> bedrijfskritische platform voor **nomineren, stemmen en awards**. Young Amstelland Connect
> wordt een omgeving voor **verbinden, ontwikkelen, inspireren en kansen creëren**. Hierdoor
> kunnen nieuwe ideeën veilig worden ontwikkeld en getest zonder risico voor het bestaande
> YAA-platform.
>
> ### Doel
>
> Young Amstelland Connect moet een digitaal ecosysteem worden waarin jong talent wordt
> verbonden met onder andere:
>
> * jongeren, genomineerden, winnaars en YAA-alumni;
> * juryleden, ambassadeurs, mentoren en experts;
> * gemeenten en jongerenwerk;
> * scholen en onderwijsinstellingen;
> * bedrijven en ondernemers;
> * maatschappelijke, culturele en sportorganisaties;
> * partners, fondsen en andere organisaties die kansen voor jongeren bieden.
>
> ### Mogelijke functionaliteiten
>
> Denk aan **gebruikers- en organisatieprofielen, talent/project showcases, challenges,
> opportunities, events, badges & achievements, nieuws/inspiratie, community-interactie,
> matching en een beheerdersdashboard**.
>
> De studenten krijgen nadrukkelijk ruimte om zelf met innovatieve ideeën te komen. De
> centrale ontwerpvraag is:
>
> **“Hoe kunnen we een digitaal ecosysteem ontwikkelen waarin jong talent uit Amstelland niet
> alleen zichtbaar wordt, maar ook duurzaam wordt verbonden met mensen, organisaties, kansen
> en maatschappelijke uitdagingen?”**
>
> ### Belangrijke randvoorwaarde
>
> Young Amstelland Connect wordt ontwikkeld als **standalone applicatie**, met een eigen
> technische omgeving. Er mogen geen wijzigingen nodig zijn aan de productieomgeving, database
> of stemfunctionaliteit van YAA. Koppelingen met YAA kunnen in eerste instantie via links
> plaatsvinden en later eventueel via een API.

## The concept diagram

`docs/client/clientBriefConcept.jpeg`, 1536 by 1024. It shows the same split as the text: the awards
platform keeps nominations, voting, jury process and gala, marked "Los van elkaar (wel
verbonden)", joined by link or reference only. Connect carries the modules profielen,
projecten, challenges, events, community, badges, nieuws and beheer & statistieken, with its
own standalone hosting, its own database and external services for mail, payments, maps and
analytics. The user groups down the right side match the Doel list in the text.

One module appears only in the diagram and not in the text: a **webshop** for T-shirts, caps
and agendas. Payments pull in external services the text does not otherwise require, so the
client decides whether it is in scope for the semester. That is question 2 of
`docs/client/clientQuestions.md`, asked in issue 10 The client's priorities for the epics. On
the board it is FR10 Webshop (not confirmed by the client), epic Webshop, and issues 81, 82 and
83 are blocked until the answer.
