import { useRef } from 'react';
import { motion, useInView } from 'framer-motion';
import { Clock, Brain, Puzzle, FileCheck, Zap } from 'lucide-react';

const features = [
  {
    id: 1,
    title: '30% времени — впустую?',
    description: 'Столько тратит средняя команда на рутинное описание задач после митингов. TaskWave AI возвращает вам это время.',
    icon: Clock,
    size: 'large',
    gradient: 'from-indigo-500/20 to-purple-500/20',
  },
  {
    id: 2,
    title: 'Умная транскрипция',
    description: 'AI понимает сложный техстек и контекст. Больше никаких ошибок в названиях фреймворков или API.',
    icon: Brain,
    size: 'medium',
    gradient: 'from-purple-500/20 to-pink-500/20',
  },
  {
    id: 3,
    title: 'Интеграции',
    description: '',
    icon: Puzzle,
    size: 'small',
    gradient: 'from-blue-500/20 to-indigo-500/20',
    isIntegrations: true,
  },
  {
    id: 4,
    title: 'Готов к работе',
    description: 'На выходе — идеальные User Stories, Эпики и Acceptance Criteria. Просто скопируйте в таск-трекер.',
    icon: FileCheck,
    size: 'medium',
    gradient: 'from-emerald-500/20 to-teal-500/20',
  },
];

const integrations = [
  { name: 'Zoom', icon: 'M12 2C6.48 2 2 6.48 2 12s4.48 10 10 10 10-4.48 10-10S17.52 2 12 2zm-1 14H9V8h2v8zm4 0h-2V8h2v8z' },
  { name: 'Meet', icon: 'M12 2C6.48 2 2 6.48 2 12s4.48 10 10 10 10-4.48 10-10S17.52 2 12 2zm-2 15l-5-5 1.41-1.41L10 14.17l7.59-7.59L19 8l-9 9z' },
  { name: 'Teams', icon: 'M12 2C6.48 2 2 6.48 2 12s4.48 10 10 10 10-4.48 10-10S17.52 2 12 2zm0 3c1.66 0 3 1.34 3 3s-1.34 3-3 3-3-1.34-3-3 1.34-3 3-3zm0 14.2c-2.5 0-4.71-1.28-6-3.22.03-1.99 4-3.08 6-3.08 1.99 0 5.97 1.09 6 3.08-1.29 1.94-3.5 3.22-6 3.22z' },
  { name: 'Slack', icon: 'M12 2C6.48 2 2 6.48 2 12s4.48 10 10 10 10-4.48 10-10S17.52 2 12 2zm4.64 6.8c-.15 1.58-.8 5.42-1.13 7.19-.14.75-.42 1-.68 1.03-.58.05-1.02-.38-1.58-.75-.88-.58-1.38-.94-2.23-1.5-.99-.65-.35-1.01.22-1.59.15-.15 2.71-2.48 2.76-2.69a.2.2 0 0 0-.05-.18c-.06-.05-.14-.03-.21-.02-.09.02-1.49.95-4.22 2.79-.4.27-.76.41-1.08.4-.36-.01-1.04-.2-1.55-.37-.63-.2-1.12-.31-1.08-.66.02-.18.27-.36.74-.55 2.92-1.27 4.86-2.11 5.83-2.51 2.78-1.16 3.35-1.36 3.73-1.36.08 0 .27.02.39.12.1.08.13.19.14.27-.01.06.01.24 0 .38z' },
];

function BorderBeamCard({ children, className = '' }: { children: React.ReactNode; className?: string }) {
  return (
    <div className={`relative group ${className}`}>
      {/* Border beam effect */}
      <div className="absolute -inset-[1px] rounded-3xl opacity-0 group-hover:opacity-100 transition-opacity duration-500 overflow-hidden">
        <div 
          className="absolute inset-0 animate-border-beam"
          style={{
            background: 'linear-gradient(90deg, transparent, rgba(99, 102, 241, 0.8), rgba(168, 85, 247, 0.8), transparent)',
            backgroundSize: '200% 100%',
          }}
        />
      </div>
      {children}
    </div>
  );
}

export function BentoGrid() {
  const ref = useRef(null);
  const isInView = useInView(ref, { once: true, margin: '-100px' });

  return (
    <section ref={ref} className="relative py-24 px-6">
      <div className="max-w-7xl mx-auto">
        {/* Section Header */}
        <motion.div
          initial={{ opacity: 0, y: 30 }}
          animate={isInView ? { opacity: 1, y: 0 } : {}}
          transition={{ duration: 0.8, ease: [0.16, 1, 0.3, 1] }}
          className="text-center mb-16"
        >
          <h2 className="text-3xl sm:text-4xl md:text-5xl font-bold mb-4">
            Возможности, которые <span className="gradient-text">меняют правила</span>
          </h2>
          <p className="text-white/50 text-lg max-w-2xl mx-auto">
            TaskWave AI объединяет передовые технологии для максимальной продуктивности вашей команды
          </p>
        </motion.div>

        {/* Bento Grid */}
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6 auto-rows-fr">
          {features.map((feature, index) => {
            const Icon = feature.icon;
            const isLarge = feature.size === 'large';
            const isSmall = feature.size === 'small';

            return (
              <motion.div
                key={feature.id}
                initial={{ opacity: 0, y: 50, scale: 0.9 }}
                animate={isInView ? { opacity: 1, y: 0, scale: 1 } : {}}
                transition={{
                  duration: 0.8,
                  delay: index * 0.1,
                  ease: [0.16, 1, 0.3, 1],
                }}
                className={`
                  ${isLarge ? 'md:col-span-2 md:row-span-2' : ''}
                  ${isSmall ? 'md:col-span-1' : ''}
                  ${!isLarge && !isSmall ? 'md:col-span-1 md:row-span-1' : ''}
                `}
              >
                <BorderBeamCard className="h-full">
                  <motion.div
                    className={`
                      relative h-full glass-card p-6 sm:p-8 overflow-hidden
                      ${isLarge ? 'min-h-[320px]' : 'min-h-[200px]'}
                    `}
                    whileHover={{ y: -5 }}
                    transition={{ type: 'spring', stiffness: 400, damping: 17 }}
                  >
                    {/* Gradient Background */}
                    <div 
                      className={`absolute inset-0 bg-gradient-to-br ${feature.gradient} opacity-50`}
                    />
                    
                    {/* Content */}
                    <div className="relative z-10 h-full flex flex-col">
                      {/* Icon */}
                      <div className="mb-4">
                        <div className="w-12 h-12 rounded-xl bg-white/10 flex items-center justify-center">
                          <Icon className="w-6 h-6 text-indigo-400" />
                        </div>
                      </div>

                      {/* Title */}
                      <h3 className={`font-bold text-white mb-3 ${isLarge ? 'text-2xl sm:text-3xl' : 'text-xl'}`}>
                        {feature.title}
                      </h3>

                      {/* Description or Integrations */}
                      {feature.isIntegrations ? (
                        <div className="flex-1 flex items-center justify-center">
                          <div className="grid grid-cols-2 gap-4">
                            {integrations.map((integration, i) => (
                              <motion.div
                                key={i}
                                className="w-14 h-14 rounded-xl bg-white/5 border border-white/10 flex items-center justify-center"
                                whileHover={{ scale: 1.1, backgroundColor: 'rgba(255,255,255,0.15)' }}
                                transition={{ type: 'spring', stiffness: 400, damping: 17 }}
                              >
                                <svg 
                                  viewBox="0 0 24 24" 
                                  className="w-7 h-7 fill-white/70"
                                >
                                  <path d={integration.icon} />
                                </svg>
                              </motion.div>
                            ))}
                          </div>
                        </div>
                      ) : (
                        <p className="text-white/60 text-sm sm:text-base leading-relaxed flex-1">
                          {feature.description}
                        </p>
                      )}

                      {/* Decorative Element */}
                      {isLarge && (
                        <div className="mt-6 flex items-center gap-2">
                          <Zap className="w-5 h-5 text-indigo-400" />
                          <span className="text-sm text-white/50">Экономия до 10 часов в неделю</span>
                        </div>
                      )}
                    </div>

                    {/* Hover Sheen Effect */}
                    <div className="absolute inset-0 opacity-0 group-hover:opacity-100 transition-opacity duration-500 pointer-events-none">
                      <div 
                        className="absolute inset-0 bg-gradient-to-r from-transparent via-white/5 to-transparent -translate-x-full group-hover:translate-x-full transition-transform duration-1000"
                      />
                    </div>
                  </motion.div>
                </BorderBeamCard>
              </motion.div>
            );
          })}
        </div>
      </div>
    </section>
  );
}
