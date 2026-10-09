import axe from "axe-core";

// Runs the accessibility checker axe on a rendered screen and returns every rule it breaks.
// jsdom cannot work out colours, so contrast is checked with axe in a real browser instead.
export async function findAxeViolations(
  context: Element | Document,
): Promise<string[]> {
  const results = await axe.run(context, {
    rules: { "color-contrast": { enabled: false } },
  });
  return results.violations.map(
    (violation) => `${violation.id}: ${violation.help}`,
  );
}
