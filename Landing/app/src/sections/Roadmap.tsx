import { useRef } from 'react';
import { motion, useInView } from 'framer-motion';
import { Check, Loader2, Clock, Rocket } from 'lucide-react';

const roadmapItems = [
  {
    id: 1,
    title: 'Core Engine',
    description: 'Молниеносная расшифровка и понимание контекста',
    status: 'done' as const,
    icon: Check,
    date: 'ГОТОВО',
  },
  {
    id: 2,
    title: 'Web Platform',
    description: 'Ваш персональный кабинет управления задачами',
    status: 'now' as const,
    icon: Loader2,
    date: 'СЕЙЧАС',
  },
  {
    id: 3,
    title: 'Jira & Linear Sync',
    description: 'Нативная интеграция и создание тикетов в один клик',
    status: 'soon' as const,
    icon: Clock,
    date: 'СКОРО',
  },
  {
    id: 4,
    title: 'Meeting Bot',
    description: 'AI-ассистент, который сам приходит на встречу вместо вас',
    status: 'planned' as const,
    icon: Rocket,
    date: 'В ПЛАНАХ',
  },
];

const statusConfig = {
  done: {
    color: 'text-emerald-400',
    bgColor: 'bg-emerald-500/20',
    borderColor: 'border-emerald-500/30',
    lineColor: 'bg-emerald-500',
  },
  now: {
    color: 'text-indigo-400',
    bgColor: 'bg-indigo-500/20',
    borderColor: 'border-indigo-500/30',
    lineColor: 'bg-indigo-500',
  },
  soon: {
    color: 'text-purple-400',
    bgColor: 'bg-purple-500/20',
    borderColor: 'border-purple-500/30',
    lineColor: 'bg-purple-500',
  },
  planned: {
    color: 'text-white/40',
    bgColor: 'bg-white/5',
    borderColor: 'border-white/10',
    lineColor: 'bg-white/20',
  },
};

export function Roadmap() {
  const ref = useRef(null);
  const isInView = useInView(ref, { once: true, margin: '-100px' });

  return (
    <section ref={ref} className="relative py-24 px-6">
      {/* Background Glow */}
      <div className="absolute inset-0 z-0">
        <div 
          className="absolute top-1/2 left-1/2 -translate-x-1/2 -translate-y-1/2 w-[800px] h-[800px] rounded-full opacity-20"
          style={{
            background: 'radial-gradient(circle, rgba(99, 102, 241, 0.3) 0%, transparent 60%)',
          }}
        />
      </div>

      <div className="max-w-4xl mx-auto relative z-10">
        {/* Section Header */}
        <motion.div
          initial={{ opacity: 0, y: 30 }}
          animate={isInView ? { opacity: 1, y: 0 } : {}}
          transition={{ duration: 0.8, ease: [0.16, 1, 0.3, 1] as const }}
          className="text-center mb-16"
        >
          <h2 className="text-3xl sm:text-4xl md:text-5xl font-bold mb-4">
            Путь <span className="gradient-text">развития</span>
          </h2>
          <p className="text-white/50 text-lg max-w-2xl mx-auto">
            Мы постоянно улучшаем TaskWave AI, чтобы он соответствовал вашим потребностям
          </p>
        </motion.div>

        {/* Timeline */}
        <div className="relative">
          {/* Vertical Line */}
          <div className="absolute left-8 md:left-1/2 top-0 bottom-0 w-[2px] -translate-x-1/2">
            {/* Background line */}
            <div className="absolute inset-0 bg-white/10" />
            {/* Animated progress line */}
            <motion.div
              className="absolute top-0 left-0 w-full bg-gradient-to-b from-emerald-500 via-indigo-500 to-purple-500"
              initial={{ height: '0%' }}
              animate={isInView ? { height: '60%' } : {}}
              transition={{ duration: 1.5, ease: [0.16, 1, 0.3, 1] as const }}
            />
          </div>

          {/* Items */}
          <div className="space-y-12">
            {roadmapItems.map((item, index) => {
              const Icon = item.icon;
              const config = statusConfig[item.status];
              const isEven = index % 2 === 0;

              return (
                <motion.div
                  key={item.id}
                  initial={{ opacity: 0, x: isEven ? -50 : 50 }}
                  animate={isInView ? { opacity: 1, x: 0 } : {}}
                  transition={{
                    duration: 0.8,
                    delay: index * 0.15,
                    ease: [0.16, 1, 0.3, 1] as const,
                  }}
                  className={`relative flex items-center ${isEven ? 'md:flex-row' : 'md:flex-row-reverse'}`}
                >
                  {/* Content Card */}
                  <div className={`ml-20 md:ml-0 md:w-5/12 ${isEven ? 'md:pr-12 md:text-right' : 'md:pl-12'}`}>
                    <motion.div
                      className={`glass-card p-6 ${config.borderColor}`}
                      whileHover={{ scale: 1.02 }}
                      transition={{ type: 'spring', stiffness: 400, damping: 17 }}
                    >
                      {/* Status Badge */}
                      <div className={`inline-flex items-center gap-2 px-3 py-1 rounded-full ${config.bgColor} ${config.color} text-xs font-semibold mb-3`}>
                        <Icon className={`w-3 h-3 ${item.status === 'now' ? 'animate-spin' : ''}`} />
                        {item.date}
                      </div>

                      {/* Title */}
                      <h3 className="text-xl font-bold text-white mb-2">
                        {item.title}
                      </h3>

                      {/* Description */}
                      <p className="text-white/60 text-sm leading-relaxed">
                        {item.description}
                      </p>
                    </motion.div>
                  </div>

                  {/* Center Node */}
                  <div className="absolute left-8 md:left-1/2 -translate-x-1/2">
                    <motion.div
                      className={`relative w-4 h-4 rounded-full ${config.lineColor}`}
                      animate={item.status === 'now' ? {
                        boxShadow: [
                          '0 0 0 0 rgba(99, 102, 241, 0.4)',
                          '0 0 0 10px rgba(99, 102, 241, 0)',
                        ],
                      } : {}}
                      transition={{
                        duration: 2,
                        repeat: Infinity,
                        ease: 'easeOut',
                      }}
                    >
                      {/* Radar ping for current item */}
                      {item.status === 'now' && (
                        <>
                          <motion.div
                            className="absolute inset-0 rounded-full bg-indigo-500"
                            animate={{
                              scale: [1, 2.5],
                              opacity: [0.5, 0],
                            }}
                            transition={{
                              duration: 2,
                              repeat: Infinity,
                              ease: 'easeOut',
                            }}
                          />
                          <motion.div
                            className="absolute inset-0 rounded-full bg-indigo-500"
                            animate={{
                              scale: [1, 2.5],
                              opacity: [0.5, 0],
                            }}
                            transition={{
                              duration: 2,
                              repeat: Infinity,
                              ease: 'easeOut',
                              delay: 0.5,
                            }}
                          />
                        </>
                      )}
                    </motion.div>
                  </div>

                  {/* Empty space for alternating layout */}
                  <div className="hidden md:block md:w-5/12" />
                </motion.div>
              );
            })}
          </div>
        </div>
      </div>
    </section>
  );
}
