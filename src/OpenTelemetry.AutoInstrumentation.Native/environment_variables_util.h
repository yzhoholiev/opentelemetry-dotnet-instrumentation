#ifndef OTEL_CLR_PROFILER_ENVIRONMENT_VARIABLES_UTIL_H_
#define OTEL_CLR_PROFILER_ENVIRONMENT_VARIABLES_UTIL_H_

#include "environment_variables.h"
#include "string.h"
#include "util.h"

#define CheckIfTrue(EXPR)                                               \
  static int sValue = -1;                                               \
  if (sValue == -1) {                                                   \
    const auto envValue = EXPR;                                         \
    sValue = envValue == WStr("1") || envValue == WStr("true") ? 1 : 0; \
  }                                                                     \
  return sValue == 1;

#define CheckIfFalse(EXPR)                                               \
  static int sValue = -1;                                                \
  if (sValue == -1) {                                                    \
    const auto envValue = EXPR;                                          \
    sValue = envValue == WStr("0") || envValue == WStr("false") ? 1 : 0; \
  }                                                                      \
  return sValue == 1;

#define ToBooleanWithDefault(EXPR, DEFAULT)                          \
  static int sValue = -1;                                            \
  if (sValue == -1) {                                                \
    const auto envValue = EXPR;                                      \
    if (envValue == WStr("1") || envValue == WStr("true")) {         \
      sValue = 1;                                                    \
    } else if (envValue == WStr("0") || envValue == WStr("false")) { \
      sValue = 0;                                                    \
    } else {                                                         \
      sValue = DEFAULT;                                              \
    }                                                                \
  }                                                                  \
  return sValue == 1;

namespace trace {

bool DisableOptimizations() {
  CheckIfTrue(GetEnvironmentValue(environment::clr_disable_optimizations));
}

bool EnableInlining() {
  ToBooleanWithDefault(GetEnvironmentValue(environment::clr_enable_inlining), true);
}

bool IsNGENEnabled() {
  ToBooleanWithDefault(GetEnvironmentValue(environment::clr_enable_ngen),
                       false);
}

bool IsDebugEnabled() {
  CheckIfTrue(GetEnvironmentValue(environment::debug_enabled));
}

bool IsDumpILRewriteEnabled() {
  CheckIfTrue(GetEnvironmentValue(environment::dump_il_rewrite_enabled));
}

bool IsTracingDisabled() {
  CheckIfFalse(GetEnvironmentValue(environment::clr_profiler_enabled));
}

bool IsAzureAppServices() {
  CheckIfTrue(GetEnvironmentValue(environment::azure_app_services));
}

}  // namespace trace

#endif  // OTEL_CLR_PROFILER_ENVIRONMENT_VARIABLES_UTIL_H_