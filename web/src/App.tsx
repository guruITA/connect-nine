import { Route, Routes } from "react-router";
import SiteFooter from "./components/SiteFooter";
import TopBar from "./components/TopBar";
import CreateAccount from "./pages/CreateAccount";
import CreateProfile from "./pages/CreateProfile";
import Home from "./pages/Home";
import MemberProfile from "./pages/MemberProfile";
import MyProfile from "./pages/MyProfile";

export default function App() {
  return (
    <>
      <TopBar />
      <main>
        <Routes>
          <Route path="/" element={<Home />} />
          <Route path="/create-account" element={<CreateAccount />} />
          {/* Goes inside RequireLogin when log in (#15) adds it. */}
          <Route path="/profile/new" element={<CreateProfile />} />
          {/* Goes inside RequireLogin when log in (#15) adds it. */}
          <Route path="/profile" element={<MyProfile />} />
          {/* Goes inside RequireLogin when log in (#15) adds it. */}
          <Route path="/members/:id" element={<MemberProfile />} />
        </Routes>
      </main>
      <SiteFooter />
    </>
  );
}
