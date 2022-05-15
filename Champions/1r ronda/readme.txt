50 generacions fetes:
 -Amb penalitzacions
 -6 accions: _, s, d, t, p, e
 -Sense donar recompensa pel temps
 -Sense pegar-se amb la coded

Parametres:

public static float new_node_mutation_rate = 0.03f;
public static float new_link_mutation_rate = 0.05f;
public static int dropoff = 3;
public static int max_len = 0;
public static float step = 2.5f;
public static int species_pool_size = 3;
public static float bloodrate = 1.0f;
public static float survival_threshold = 0.334f;
public static float distance_thld = 6.0f;
public static float c1 = 2.0f; // Disjoint
public static float c2 = 2.0f; // Excess
public static float c3 = 1.0f; // Weight
public const int N_INPUTS = 12;
public const int N_OUTPUTS = 6;
public static string activation_function = "relu";
public static string inner_activation_function = "relu";