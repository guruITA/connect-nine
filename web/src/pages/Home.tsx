import ClosingCall from "../components/home/ClosingCall";
import FoundationStory from "../components/home/FoundationStory";
import HomeHero from "../components/home/HomeHero";
import HowItWorks from "../components/home/HowItWorks";
import ForOrganisations from "../components/home/ForOrganisations";
import WaysIn from "../components/home/WaysIn";

// The home page for a visitor, as the Figma frame "A01 Home, visitor" draws it.
export default function Home() {
  return (
    <>
      <HomeHero />
      <HowItWorks />
      <WaysIn />
      <FoundationStory />
      <ForOrganisations />
      <ClosingCall />
    </>
  );
}
