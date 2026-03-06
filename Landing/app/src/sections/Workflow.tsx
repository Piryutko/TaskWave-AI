import { useRef, useEffect, useState } from 'react';
import { motion, useInView, useScroll, useTransform } from 'framer-motion';
import { Upload, Sparkles, Rocket } from 'lucide-react';

const steps = [
  {
    id: 1,
    title: 'Захват',
    subtitle: 'Capture',
    description: 'Загрузите запись. AI ловит каждое важное слово, не упуская нюансов.',
    icon: Upload,
    color: 'from-indigo-500 to-blue-500',
  },
  {
    id: 2,
    title: 'Очистка',
    subtitle: 'Refine',
    description: 'Убираем "воду", структурируем хаос и выделяем реальные действия.',
    icon: Sparkles,
    color: 'from-purple-500 to-pink-500',
  },
  {
    id: 3,
    title: 'Запуск',
    subtitle: 'Deploy',
    description: 'Задачи мгновенно готовы к импорту в ваш рабочий процесс.',
    icon: Rocket,
    color: 'from-emerald-500 to-teal-500',
  },
];

export function Workflow() {
  const ref = useRef(null);
  const containerRef = useRef(null);
  const isInView = useInView(ref, { once: true, margin: '-100px' });
  const [lineProgress, setLineProgress] = useState(0);

  const { scrollYProgress } = useScroll({
    target: containerRef,
    offset: ['start end', 'end start'],
  });

  const lineHeight = useTransform(scrollYProgress, [0, 0.5], ['0%', '100%']);

  useEffect(() => {
    const unsubscribe = scrollYProgress.on('change', (latest) => {
      setLineProgress(latest);
    });
    return () => unsubscribe();
  }, [scrollYProgress]);

  return (
    <section ref={containerRef} className="relative py-24 px-6 overflow-hidden">
      {/* Background Glow */}
      <div className="absolute inset-0 z-0">
        <div 
          className="absolute top-1/2 left-1/2 -translate-x-1/2 -translate-y-1/2 w-[600px] h-[600px] rounded-full opacity-30"
          style={{
            background: 'radial-gradient(circle, rgba(99, 102, 241, 0.2) 0%, transparent 70%)',
          }}
        />
      </div>

      <div ref={ref} className="max-w-6xl mx-auto relative z-10">
        {/* Section Header */}
        <motion.div
          initial={{ opacity: 0, y: 30 }}
          animate={isInView ? { opacity: 1, y: 0 } : {}}
          transition={{ duration: 0.8, ease: [0.16, 1, 0.3, 1] }}
          className="text-center mb-20"
        >
          <h2 className="text-3xl sm:text-4xl md:text-5xl font-bold mb-4">
            Как это <span className="gradient-text">работает</span>
          </h2>
          <p className="text-white/50 text-lg max-w-2xl mx-auto">
            Три простых шага от записи встречи до готовых задач
          </p>
        </motion.div>

        {/* Workflow Cards with Connector */}
        <div className="relative">
          {/* Glowing Connector Line - Desktop */}
          <div className="hidden lg:block absolute top-1/2 left-0 right-0 h-[2px] -translate-y-1/2 z-0">
            {/* Background line */}
            <div className="absolute inset-0 bg-white/10" />
            {/* Animated glow line */}
            <motion.div
              className="absolute left-0 top-0 h-full bg-gradient-to-r from-indigo-500 via-purple-500 to-emerald-500"
              style={{ width: lineHeight }}
            />
            {/* Glow effect */}
            <motion.div
              className="absolute left-0 top-1/2 -translate-y-1/2 h-8 bg-gradient-to-r from-indigo-500/50 via-purple-500/50 to-emerald-500/50 blur-xl"
              style={{ width: lineHeight }}
            />
          </div>

          {/* Cards */}
          <div className="grid grid-cols-1 lg:grid-cols-3 gap-8 lg:gap-12">
            {steps.map((step, index) => {
              const Icon = step.icon;
              const isActive = lineProgress > (index + 1) * 0.25;

              return (
                <motion.div
                  key={step.id}
                  initial={{ opacity: 0, y: 50 }}
                  animate={isInView ? { opacity: 1, y: 0 } : {}}
                  transition={{
                    duration: 0.8,
                    delay: index * 0.2,
                    ease: [0.16, 1, 0.3, 1],
                  }}
                  className="relative"
                >
                  {/* Card */}
                  <motion.div
                    className="relative glass-card p-8 h-full"
                    whileHover={{ y: -10, scale: 1.02 }}
                    transition={{ type: 'spring', stiffness: 400, damping: 17 }}
                  >
                    {/* Step Number */}
                    <div className="absolute -top-4 left-8">
                      <motion.div
                        className={`w-8 h-8 rounded-full bg-gradient-to-r ${step.color} flex items-center justify-center text-sm font-bold text-white`}
                        animate={isActive ? { scale: [1, 1.2, 1] } : {}}
                        transition={{ duration: 0.5 }}
                      >
                        {step.id}
                      </motion.div>
                    </div>

                    {/* Icon */}
                    <div className="mb-6 mt-4">
                      <motion.div
                        className={`w-16 h-16 rounded-2xl bg-gradient-to-r ${step.color} p-[1px]`}
                        whileHover={{ rotate: 5 }}
                      >
                        <div className="w-full h-full rounded-2xl bg-black/80 flex items-center justify-center">
                          <Icon className="w-8 h-8 text-white" />
                        </div>
                      </motion.div>
                    </div>

                    {/* Content */}
                    <div className="space-y-3">
                      <div className="flex items-center gap-2">
                        <h3 className="text-xl font-bold text-white">{step.title}</h3>
                        <span className="text-sm text-white/40">{step.subtitle}</span>
                      </div>
                      <p className="text-white/60 leading-relaxed">
                        {step.description}
                      </p>
                    </div>

                    {/* Decorative Corner */}
                    <div className={`absolute top-0 right-0 w-20 h-20 bg-gradient-to-bl ${step.color} opacity-10 rounded-tr-3xl`} />
                  </motion.div>

                  {/* Data Packet Animation */}
                  {index < steps.length - 1 && (
                    <motion.div
                      className="hidden lg:block absolute top-1/2 -right-6 w-3 h-3 rounded-full bg-indigo-500 z-10"
                      style={{
                        boxShadow: '0 0 10px rgba(99, 102, 241, 0.8)',
                      }}
                      animate={{
                        x: [0, 20, 0],
                        opacity: [0.5, 1, 0.5],
                      }}
                      transition={{
                        duration: 2,
                        repeat: Infinity,
                        delay: index * 0.5,
                      }}
                    />
                  )}
                </motion.div>
              );
            })}
          </div>
        </div>

        {/* Bottom CTA */}
        <motion.div
          initial={{ opacity: 0, y: 30 }}
          animate={isInView ? { opacity: 1, y: 0 } : {}}
          transition={{ duration: 0.8, delay: 0.6, ease: [0.16, 1, 0.3, 1] }}
          className="mt-16 text-center"
        >
          <p className="text-white/50 text-sm">
            Среднее время обработки: <span className="text-indigo-400 font-semibold">2 минуты</span> на час записи
          </p>
        </motion.div>
      </div>
    </section>
  );
}
