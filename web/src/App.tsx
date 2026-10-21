import { Route, Routes } from "react-router";
import SiteFooter from "./components/SiteFooter";
import TopBar from "./components/TopBar";
import CreateAccount from "./pages/CreateAccount";
import Home from "./pages/Home";

export default function App() {
  return (
    <>
      <TopBar />
      <main>
        <Routes>
          <Route path="/" element={<Home />} />
          <Route path="/create-account" element={<CreateAccount />} />
        </Routes>
      </main>
      <SiteFooter />
    </>
  );
}
