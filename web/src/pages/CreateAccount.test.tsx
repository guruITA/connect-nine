import { render, screen } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import { MemoryRouter } from "react-router";
import { beforeEach, describe, expect, it, vi } from "vitest";
import { findAxeViolations } from "../test/axe";
import { createAccount } from "../api/accounts";
import CreateAccount from "./CreateAccount";

vi.mock("../api/accounts", () => ({
  createAccount: vi.fn(),
}));

const mockedCreateAccount = vi.mocked(createAccount);

function renderPage() {
  return render(
    <MemoryRouter>
      <CreateAccount />
    </MemoryRouter>,
  );
}

describe("CreateAccount", () => {
  beforeEach(() => {
    mockedCreateAccount.mockReset();
  });

  it("has accessible fields and keeps the password hidden", () => {
    renderPage();

    expect(screen.getByLabelText("Email")).toHaveAttribute("type", "email");
    expect(screen.getByLabelText("Email")).toHaveAttribute(
      "autocomplete",
      "email",
    );
    expect(screen.getByLabelText("Password")).toHaveAttribute(
      "type",
      "password",
    );
    expect(screen.getByLabelText("Password")).toHaveAttribute(
      "autocomplete",
      "new-password",
    );
    expect(
      screen.getByRole("form", { name: "Make an account" }),
    ).toHaveAttribute("novalidate");
  });

  it("shows the busy button while the request is pending", async () => {
    let resolve: (value: { kind: "made" }) => void = () => undefined;
    mockedCreateAccount.mockReturnValue(
      new Promise((accept) => {
        resolve = accept;
      }),
    );
    const user = userEvent.setup();
    renderPage();

    await user.type(screen.getByLabelText("Email"), "sam@example.org");
    await user.type(screen.getByLabelText("Password"), "a secure password");
    await user.click(screen.getByRole("button", { name: "Make account" }));

    expect(
      screen.getByRole("button", { name: "Making your account..." }),
    ).toBeDisabled();
    resolve({ kind: "made" });
  });

  it("shows the made status and clears the password", async () => {
    mockedCreateAccount.mockResolvedValue({ kind: "made" });
    const user = userEvent.setup();
    renderPage();

    await user.type(screen.getByLabelText("Email"), "sam@example.org");
    await user.type(screen.getByLabelText("Password"), "a secure password");
    await user.click(screen.getByRole("button", { name: "Make account" }));

    expect(screen.getByRole("status")).toHaveTextContent(
      "Your account is made",
    );
    expect(screen.queryByLabelText("Password")).not.toBeInTheDocument();
  });

  it("shows field messages for invalid input", async () => {
    mockedCreateAccount.mockResolvedValue({
      kind: "invalid",
      fields: {
        email: "Fill in a valid email address",
        password: "Fill in a password",
      },
    });
    const user = userEvent.setup();
    renderPage();

    await user.click(screen.getByRole("button", { name: "Make account" }));

    expect(
      screen.getByText("Fill in a valid email address"),
    ).toBeInTheDocument();
    expect(screen.getByText("Fill in a password")).toBeInTheDocument();
    expect(screen.getByLabelText("Email")).toHaveAttribute(
      "aria-describedby",
      "email-error",
    );
    expect(screen.getByLabelText("Password")).toHaveAttribute(
      "aria-describedby",
      "password-error",
    );
  });

  it("shows the taken message next to the email field", async () => {
    mockedCreateAccount.mockResolvedValue({ kind: "taken" });
    const user = userEvent.setup();
    renderPage();

    await user.click(screen.getByRole("button", { name: "Make account" }));

    expect(
      screen.getByText("This email address already has an account"),
    ).toBeInTheDocument();
    expect(screen.getByLabelText("Email")).toHaveAttribute(
      "aria-describedby",
      "email-error",
    );
  });

  it.each(["not reachable"] as const)(
    "shows the not reachable alert",
    async (kind) => {
      mockedCreateAccount.mockResolvedValue({ kind });
      const user = userEvent.setup();
      renderPage();

      await user.click(screen.getByRole("button", { name: "Make account" }));

      expect(screen.getByRole("alert")).toHaveTextContent(
        "Connect is not reachable right now, try again later",
      );
    },
  );

  it("has no axe violations in the empty state", async () => {
    const { container } = renderPage();
    expect(await findAxeViolations(container)).toEqual([]);
  });
});
