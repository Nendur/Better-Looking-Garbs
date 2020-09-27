import os
import re

scripted_triggers = {}

scripted_triggers_path = "betterlookinggarbs/common/scripted_triggers/"

rewrite_files = [
	"betterlookinggarbs/interface/portrait_properties/00_portraits_background.txt",
	"betterlookinggarbs/interface/portrait_properties/01_hair.txt",
	"betterlookinggarbs/interface/portrait_properties/02_background_items.txt",
	"betterlookinggarbs/interface/portrait_properties/03_clothes.txt",
	"betterlookinggarbs/interface/portrait_properties/04_beard.txt",
	"betterlookinggarbs/interface/portrait_properties/05_headgear.txt",
	"betterlookinggarbs/interface/portrait_properties/06_imprisoned.txt",
	"betterlookinggarbs/interface/portrait_properties/07_scars.txt",
	"betterlookinggarbs/interface/portrait_properties/08_red_dots.txt",
	"betterlookinggarbs/interface/portrait_properties/09_boils.txt",
	"betterlookinggarbs/interface/portrait_properties/10_diseases.txt",
	"betterlookinggarbs/interface/portrait_properties/12_disfigured.txt",
	"betterlookinggarbs/interface/portrait_properties/13_blindfold.txt",
	"betterlookinggarbs/interface/portrait_properties/14_makeup.txt",
	"betterlookinggarbs/interface/portrait_properties/15_lipstick.txt",
	"betterlookinggarbs/interface/portrait_properties/17_child_background.txt",
	"betterlookinggarbs/interface/portrait_properties/18_child_portrait.txt",
	"betterlookinggarbs/interface/portrait_properties/19_special_crown.txt",
	"betterlookinggarbs/interface/portrait_properties/21_physique.txt",
	"betterlookinggarbs/interface/portrait_properties/22_pale.txt",
	"betterlookinggarbs/interface/portrait_properties/23_wounded.txt",
	"betterlookinggarbs/interface/portrait_properties/24_harelip.txt",
	"betterlookinggarbs/interface/portrait_properties/25_scars.txt",
	"betterlookinggarbs/interface/portrait_properties/26_scars.txt",
	"betterlookinggarbs/interface/portrait_properties/27_bloodsplatter.txt",
	"betterlookinggarbs/interface/portrait_properties/30_possessed.txt",
	"betterlookinggarbs/interface/portrait_properties/34_special_helmet.txt",
	"betterlookinggarbs/interface/portrait_properties/35_special_mask.txt",
	"betterlookinggarbs/interface/portrait_properties/36_scepter.txt",
	"betterlookinggarbs/interface/portrait_properties/38_background_frame.txt",
	"betterlookinggarbs/interface/portrait_properties/39_weapons.txt",
	"betterlookinggarbs/interface/portrait_properties/40_weapons_left.txt",
	"betterlookinggarbs/interface/portrait_properties/42_background_furniture.txt",
	"dlc074/interface/portrait_properties/31_male_overlayer.txt",
	"dlc074/interface/portrait_properties/32_female_overlayer.txt",
	"dlc074/interface/portrait_properties/33_female_underlayer.txt",
	"dlc074/interface/portrait_properties/43_helmet.txt",
	"betterlookinggarbs/interface/portraits/portraits_merchants.gfx",
	"betterlookinggarbs/interface/portraits/HF_society_clothes.gfx",
	"betterlookinggarbs/interface/portraits/religious_clothing.gfx",
	"betterlookinggarbs/interface/portraits/special_clothing.gfx",
	"dlc070/interface/portraits/society_clothes.gfx",
	"dlc070/interface/portraits/society_indian.gfx",
	"dlc070/interface/portraits/society_monastic.gfx",
	"dlc074/interface/portraits/HF_warrior_lodges_clothes.gfx"
]

replaced_triggers = {
	"blg_mount_horse_charger": [],
	"blg_mount_horse_destrier": [],
	"blg_mount_horse": [],
	"blg_mount_horse_armor_1": [],
	"blg_mount_horse_armor_2": [],
	"blg_mount_horse_armor_3": [],
	"blg_mount_horse_black": [],
	"blg_mount_horse_white": [],
	"blg_mount_bear": [],
	"blg_mount_camel": [],
	"blg_mount_elephant": [],
	"blg_mount_lion": [],
	"blg_mount_lizard": [],
	"blg_mount_polarbear": [],
	"blg_mount_reindeer": [],
	"blg_mount_tiger": [],
	"blg_mount_yak": [],
	"blg_mount_wolf": [],
	"blg_adult": ["practical_age >= 16"],
	"blg_artifact_candle": ["always = no"],
	"blg_headgear_military_on_rule": [],
	"blg_headgear_military_off_rule": ["has_game_rule = { name = martial_headgear value = off }"],
	"blg_weapon_dagger_flag": [],
	"blg_mummy": []
}

delete_list = [
	#"always = no",
	"has_character_modifier = child_cat",
	"has_character_modifier = child_dog",
	"has_artifact_flag = harp",
	"has_artifact_flag = scroll",
	"has_artifact_flag = flail",
	"has_artifact_flag = greatsword",
	"has_artifact_flag = hammer",
	"has_artifact_flag = polearm",
	"has_artifact_flag = scimitar",
	"has_artifact_flag = trident",
	"religion_openly_in_celtic_subgroup_trigger = yes",
	"religion_openly_egyptian_or_reformed_trigger = yes",
	"portrait_has_trait = blgcaon",
	"portrait_has_trait = blgquesting",
	"portrait_has_trait = blgcaoff",
	"has_character_flag = chivalry_duel_ongoing",
	"has_character_flag = blgcamaskoff",
	"has_artifact_flag = offhand",
	"has_game_rule = { name = blg_children value = 16 }",
	"has_game_rule = { name = blg_children value = 16noscale }",
	"has_game_rule = { name = blg_children value = 15 }",
	"has_game_rule = { name = blg_children value = 15noscale }",
	"has_game_rule = { name = blg_children value = 14 }",
	"has_game_rule = { name = blg_children value = 14noscale }",
	"has_game_rule = { name = blg_helmet value = off }",
	"has_game_rule = { name = blg_helmet value = none }",
	"has_game_rule = { name = blg_wounds value = off }",
	"has_game_rule = { name = blg_wounds value = nowounds }",
	"has_game_rule = { name = blg_wounds value = nomasks }",
	"has_game_rule = { name = blg_wounds value = postmortem }",
	"has_game_rule = { name = blg_disease value = off }",
	"has_game_rule = { name = blg_disease value = postmortem }",
	"has_game_rule = { name = blg_traits value = notraits }",
	"has_game_rule = { name = blg_traits value = nolunatic }",
	"has_game_rule = { name = blg_traits value = nofair }",
	"has_game_rule = { name = blg_traits value = off }",
	"has_game_rule = { name = blg_artifacts value = off }",
	"has_game_rule = { name = blg_artifacts value = rare }",
	"has_game_rule = { name = blg_artifacts value = weapons }",
	"NOT = { has_game_rule = { name = blg_disease value = off } }",
	"NOT = { has_game_rule = { name = blg_artifacts value = off } }",
	"NOT = { has_game_rule = { name = blg_helmet value = none } }",
	"graphical_culture = somaligfx",
	"graphical_culture = muslimgfx",
	"graphical_culture = hellenicgfx",
	"graphical_culture = siciliangfx",
	"graphical_culture = bretongfx",
	"graphical_culture = romanobritishgfx",
	"graphical_culture = centralasiangfx",
	"graphical_culture = hunnicgfx",
	"graphical_culture = steppegfx",
	"graphical_culture = siberiangfx",
	"graphical_culture = dutchgfx",
	"graphical_culture = westerngfx",
	"graphical_culture = copticgfx",
	"graphical_culture = oldfrankishgfx",
	"graphical_culture = basquegfx",
	"graphical_culture = celtiberiangfx",
	"graphical_culture = indusgfx",
	"graphical_culture = semitegfx",
	"graphical_culture = lombardgfx",
	"graphical_culture = visigothgfx",
	"graphical_culture = khitangfx",
	"graphical_culture = scandinaviangfx",
	"graphical_culture = norsegaelgfx",
	"graphical_culture = gallicgfx",
	"graphical_culture = afghangfx",
	"graphical_culture = vlachgfx",
	"graphical_culture = romanobalkangfx",
	"graphical_culture = alangfx",
	"graphical_culture = sogdiangfx",
	"graphical_culture = centralindiangfx",
	"graphical_culture = assamesegfx",
	"graphical_culture = tangutgfx",
	"graphical_culture = pecheneggfx",
	"graphical_culture = avargfx",
	"graphical_culture = balticgfx",
	"graphical_culture = finnishgfx",
	"graphical_culture = centralafricangfx",
	"graphical_culture = bohemiangfx",
	"graphical_culture = pomeraniangfx",
	"graphical_culture = georgiangfx",
	"graphical_culture = ashkenazigfx",
	"graphical_culture = nubiangfx",
	"graphical_culture = kurdishgfx",
	"graphical_culture = tochariangfx",
	"graphical_culture = khmergfx",
	"graphical_culture = polarbeargfx",
	"graphical_culture = blackbeargfx"
]

for filename in os.listdir(scripted_triggers_path):
	with open(scripted_triggers_path + filename) as scriptfile:
		for line in scriptfile:
			code = line.strip()

			if code == "" or code[0] == '#':
				pass

			elif re.match("[a-z0-9_]+ = {", line):
				trigger_name = line.split()[0]
				trigger_code = []
			elif line[0] == '}':
				if trigger_name not in replaced_triggers:
					scripted_triggers[trigger_name] = trigger_code
				else:
					scripted_triggers[trigger_name] = replaced_triggers[trigger_name]

			elif code not in delete_list:
				trigger_code.append(code)


def write_scripted_trigger(parent_scope, script, indentation, negated):
	scope_stack = ["AND"]
	code_stack = [[]]
	for line in script:
		code = line.split()

		if code[0] in scripted_triggers:
			code_stack[-1].extend(write_scripted_trigger(scope_stack[-1], scripted_triggers[code[0]], 0, code[2] == "no"))

		elif len(code) >= 3 and code[1] == '=' and code[2] == '{' and not code[-1] == '}':
			scope_stack.append(code[0])
			code_stack.append([])

		elif len(code) >= 1 and code[0] == '}':
			if len(code_stack[-1]) > 0:
				if len(code_stack[-1]) == 1 and (scope_stack[-1] == "OR" or scope_stack[-1] == "AND"):
					code_stack[-2].append((len(scope_stack) - 2) * '\t' + code_stack[-1][0].strip())
				else:
					code_stack[-2].append((len(scope_stack) - 2) * '\t' + scope_stack[-1] + " = {")
					for s in code_stack[-1]:
						code_stack[-2].append((len(scope_stack) - 2) * '\t' + s)
					code_stack[-2].append((len(scope_stack) - 2) * '\t' + "}")
			scope_stack.pop()
			code_stack.pop()

		else:
			code_stack[-1].append((len(scope_stack) - 1) * '\t' + line)

	result = []
	if len(code_stack[0]) > 0:
		simple_or = code_stack[0][0] == "OR = {" and code_stack[0][-1] == "}" and not "}" in code_stack[0][1:-1]

		if len(code_stack[0]) == 1 and not negated:
			result.append(indentation * '\t' + code_stack[0][0].strip())
		elif len(code_stack[0]) == 1 and negated:
			result.append(indentation * '\t' + "NOT = { " + code_stack[0][0].strip() + " }")
		elif simple_or and not negated and (parent_scope == "OR" or parent_scope == "NOR"):
			for s in code_stack[0][1:-1]:
				result.append((indentation - 1) * '\t' + s)
		elif simple_or and negated:
			result.append(indentation * '\t' + "NOR = {")
			for s in code_stack[0][1:]:
				result.append(indentation * '\t' + s)
		elif negated:
			result.append(indentation * '\t' + "NAND = {")
			for s in code_stack[0]:
				result.append((indentation + 1) * '\t' + s)
			result.append(indentation * '\t' + "}")
		elif parent_scope == "OR" or parent_scope == "NOR":
			result.append(indentation * '\t' + "AND = {")
			for s in code_stack[0]:
				result.append((indentation + 1) * '\t' + s)
			result.append(indentation * '\t' + "}")
		else:
			for s in code_stack[0]:
				result.append(indentation * '\t' + s)
	return result


for filename in rewrite_files:
	with open(filename) as input_file, open(re.sub("(betterlookinggarbs)|(dlc[a-z0-9]*)", "blgsubmods/blgccironman", filename), 'w') as output_file:
		scope_stack = []
		for line in input_file:
			if line.strip() in delete_list:
				continue

			code = line.strip().split()

			if len(code) >= 3 and re.match("blg_[a-z0-9_]+", code[0]):
				result = write_scripted_trigger(scope_stack[-1], scripted_triggers[code[0]], len(scope_stack), code[2] == "no")
				for s in result:
					output_file.write(s)
					output_file.write('\n')
				continue

			if len(code) >= 3 and code[1] == '=' and code[2] == '{' and '}' not in line:
				scope_stack.append(code[0])
			elif len(code) >= 1 and code[0] == '}':
				scope_stack.pop()
			output_file.write(line)
