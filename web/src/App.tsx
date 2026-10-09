import { Route, Routes } from "react-router";
import TopBar from "./components/TopBar";
import Home from "./pages/Home";

export default function App() {
  return (
    <>
      <TopBar />
      <main>
        <Routes>
          <Route path="/" element={<Home />} />
        </Routes>
      </main>
    </>
  );
}
