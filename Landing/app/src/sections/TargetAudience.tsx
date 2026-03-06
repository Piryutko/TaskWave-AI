import { useRef, useState } from 'react';
import { motion, useInView } from 'framer-motion';
import { UserCog, Code2, TrendingUp } from 'lucide-react';

const audiences = [
  {
    id: 1,
    title: 'Product Owners',
    description: 'Ваш личный секретарь, который никогда не спит и идеально понимает продукт.',
    icon: UserCog,
    gradient: 'from-indigo-500/30 to-purple-500/30',
    glowColor: 'rgba(99, 102, 241, 0.3)',
    benefits: ['Автоматические User Stories', 'Приоритизация задач', 'Отслеживание требований'],
  },
  {
    id: 2,
    title: 'Tech Leads',
    description: 'Забудьте о бесконечных уточнениях после груминга. Все требования уже зафиксированы.',
    icon: Code2,
    gradient: 'from-purple-500/30 to-pink-500/30',
    glowColor: 'rgba(168, 85, 247, 0.3)',
    benefits: ['Технические детали в контексте', 'Оценка сложности', 'Четкие acceptance criteria'],
  },
  {
    id: 3,
    title: 'Startup Founders',
    description: 'Скорость — ваше единственное преимущество. TaskWave AI делает вашу команду в 2 раза быстрее.',
    icon: TrendingUp,
    gradient: 'from-emerald-500/30 to-teal-500/30',
    glowColor: 'rgba(16, 185, 129, 0.3)',
    benefits: ['Экономия времени команды', 'Быстрый time-to-market', 'Фокус на продукте'],
  },
];

export function TargetAudience() {
  const ref = useRef(null);
  const isInView = useInView(ref, { once: true, margin: '-100px' });
  const [hoveredCard, setHoveredCard] = useState<number | null>(null);

  return (
    <section ref={ref} className="relative py-24 px-6">
      {/* Background Pattern */}
      <div className="absolute inset-0 z-0 opacity-20">
        <div 
          className="absolute inset-0"
          style={{
            backgroundImage: `radial-gradient(circle at 1px 1px, rgba(255,255,255,0.15) 1px, transparent 0)`,
            backgroundSize: '40px 40px',
          }}
        />
      </div>

      <div className="max-w-7xl mx-auto relative z-10">
        {/* Section Header */}
        <motion.div
          initial={{ opacity: 0, y: 30 }}
          animate={isInView ? { opacity: 1, y: 0 } : {}}
          transition={{ duration: 0.8, ease: [0.16, 1, 0.3, 1] }}
          className="text-center mb-16"
        >
          <h2 className="text-3xl sm:text-4xl md:text-5xl font-bold mb-4">
            Создан для <span className="gradient-text">профессионалов</span>
          </h2>
          <p className="text-white/50 text-lg max-w-2xl mx-auto">
            Независимо от вашей роли, TaskWave AI адаптируется под ваши нужды
          </p>
        </motion.div>

        {/* Cards Grid */}
        <div className="grid grid-cols-1 md:grid-cols-3 gap-8">
          {audiences.map((audience, index) => {
            const Icon = audience.icon;
            const isHovered = hoveredCard === audience.id;

            return (
              <motion.div
                key={audience.id}
                initial={{ opacity: 0, y: 50 }}
                animate={isInView ? { opacity: 1, y: 0 } : {}}
                transition={{
                  duration: 0.8,
                  delay: index * 0.15,
                  ease: [0.16, 1, 0.3, 1],
                }}
                onMouseEnter={() => setHoveredCard(audience.id)}
                onMouseLeave={() => setHoveredCard(null)}
              >
                <motion.div
                  className="relative h-full"
                  whileHover={{ y: -10 }}
                  transition={{ type: 'spring', stiffness: 400, damping: 17 }}
                >
                  {/* Glow Effect */}
                  <motion.div
                    className="absolute -inset-4 rounded-3xl opacity-0 transition-opacity duration-500"
                    style={{
                      background: `radial-gradient(circle, ${audience.glowColor} 0%, transparent 70%)`,
                    }}
                    animate={{ opacity: isHovered ? 1 : 0 }}
                  />

                  {/* Card */}
                  <div className="relative h-full bg-black/60 backdrop-blur-xl border border-white/10 rounded-3xl p-8 overflow-hidden">
                    {/* Aura Background */}
                    <motion.div
                      className={`absolute -top-20 -right-20 w-40 h-40 rounded-full bg-gradient-to-br ${audience.gradient}`}
                      animate={{
                        scale: isHovered ? 1.5 : 1,
                        opacity: isHovered ? 0.5 : 0.2,
                      }}
                      transition={{ duration: 0.5 }}
                    />

                    {/* Content */}
                    <div className="relative z-10">
                      {/* Icon */}
                      <motion.div
                        className="w-14 h-14 rounded-xl bg-white/5 border border-white/10 flex items-center justify-center mb-6"
                        animate={{
                          rotate: isHovered ? 5 : 0,
                        }}
                      >
                        <Icon className="w-7 h-7 text-white" />
                      </motion.div>

                      {/* Title */}
                      <h3 className="text-2xl font-bold text-white mb-4">
                        {audience.title}
                      </h3>

                      {/* Description */}
                      <p className="text-white/60 leading-relaxed mb-6">
                        {audience.description}
                      </p>

                      {/* Benefits List */}
                      <ul className="space-y-3">
                        {audience.benefits.map((benefit, i) => (
                          <motion.li
                            key={i}
                            className="flex items-center gap-3 text-sm text-white/70"
                            initial={{ opacity: 0, x: -10 }}
                            animate={isInView ? { opacity: 1, x: 0 } : {}}
                            transition={{ delay: index * 0.15 + i * 0.1 + 0.3 }}
                          >
                            <div className="w-1.5 h-1.5 rounded-full bg-indigo-400" />
                            {benefit}
                          </motion.li>
                        ))}
                      </ul>
                    </div>

                    {/* Bottom Border Glow */}
                    <motion.div
                      className="absolute bottom-0 left-0 right-0 h-[2px] bg-gradient-to-r from-transparent via-indigo-500 to-transparent"
                      animate={{
                        opacity: isHovered ? 1 : 0,
                      }}
                    />
                  </div>
                </motion.div>
              </motion.div>
            );
          })}
        </div>
      </div>
    </section>
  );
}
