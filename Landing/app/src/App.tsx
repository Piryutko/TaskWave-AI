import { Navigation } from './sections/Navigation';
import { Hero } from './sections/Hero';
import { BentoGrid } from './sections/BentoGrid';
import { Workflow } from './sections/Workflow';
import { TargetAudience } from './sections/TargetAudience';
import { Roadmap } from './sections/Roadmap';
import { Footer } from './sections/Footer';

function App() {
  return (
    <div className="relative min-h-screen bg-black text-white overflow-x-hidden">
      {/* Noise Overlay */}
      <div className="noise-overlay" />
      
      {/* Navigation */}
      <Navigation />
      
      {/* Main Content */}
      <main>
        {/* Hero Section */}
        <Hero />
        
        {/* Bento Grid Features */}
        <section id="features">
          <BentoGrid />
        </section>
        
        {/* Workflow Section */}
        <section id="workflow">
          <Workflow />
        </section>
        
        {/* Target Audience Section */}
        <section id="audience">
          <TargetAudience />
        </section>
        
        {/* Roadmap Section */}
        <section id="roadmap">
          <Roadmap />
        </section>
        
        {/* Footer */}
        <Footer />
      </main>
    </div>
  );
}

export default App;
